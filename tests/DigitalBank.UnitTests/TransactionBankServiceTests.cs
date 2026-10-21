using DigitalBank.Application.DTOs.TransactionBank;
using DigitalBank.Application.Services;
using DigitalBank.Domain.Constants;
using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Enums;
using DigitalBank.Domain.Exceptions;
using DigitalBank.Domain.Repositories;
using Moq;

namespace DigitalBank.UnitTests;

// Pruebas unitarias para la lógica de transacciones con Moq
public class TransactionBankServiceTests
{
    private readonly Mock<IBankAccountRepository> _bankAccountRepoMock = new();
    private readonly Mock<ITransactionBankRepository> _transactionRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly TransactionBankService _service;

    public TransactionBankServiceTests()
    {
        _service = new TransactionBankService(
            _bankAccountRepoMock.Object,
            _transactionRepoMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task DepositAsync_WhenAccountExists_ShouldIncreaseBalanceAndSaveTransaction()
    {
        // Arrange
        const string accountNumber = "ACC-20261020-1111";
        var account = new BankAccount(accountNumber, 200m, Guid.CreateVersion7(), "tester", "127.0.0.1");

        _bankAccountRepoMock
            .Setup(r => r.GetByAccountNumberAsync(accountNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var request = new TransactionBankDtoCreate(100m);

        // Act
        var result = await _service.DepositAsync(accountNumber, request, "tester", "127.0.0.1");

        // Assert
        Assert.Equal(300m, result.ResultingBalance);
        Assert.Equal(100m, result.Amount);
        Assert.Equal(TransactionType.Deposit.ToString(), result.Type);

        // Verificamos que se haya registrado la transacción y guardado con UnitOfWork
        _transactionRepoMock.Verify(t => t.AddAsync(It.Is<TransactionBank>(tb =>
            tb.Amount == 100m && tb.ResultingBalance == 300m && tb.Type == TransactionType.Deposit),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DepositAsync_WhenAccountDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        const string nonExistentAccount = "ACC-20261020-9999";
        _bankAccountRepoMock
            .Setup(r => r.GetByAccountNumberAsync(nonExistentAccount, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BankAccount?)null);

        var request = new TransactionBankDtoCreate(50m);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.DepositAsync(nonExistentAccount, request, "tester", "127.0.0.1"));

        Assert.Equal(ErrorMessages.BankAccountNotFound, exception.Message);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task WithdrawAsync_WhenFundsAreSufficient_ShouldDecreaseBalanceAndSaveTransaction()
    {
        // Arrange
        const string accountNumber = "ACC-20261020-2222";
        var account = new BankAccount(accountNumber, 500m, Guid.CreateVersion7(), "tester", "127.0.0.1");

        _bankAccountRepoMock
            .Setup(r => r.GetByAccountNumberAsync(accountNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var request = new TransactionBankDtoCreate(150m);

        // Act
        var result = await _service.WithdrawAsync(accountNumber, request, "tester", "127.0.0.1");

        // Assert
        Assert.Equal(350m, result.ResultingBalance);
        Assert.Equal(150m, result.Amount);
        Assert.Equal(TransactionType.Withdrawal.ToString(), result.Type);

        _transactionRepoMock.Verify(t => t.AddAsync(It.Is<TransactionBank>(tb =>
            tb.Amount == 150m && tb.ResultingBalance == 350m && tb.Type == TransactionType.Withdrawal),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WithdrawAsync_WhenFundsAreInsufficient_ShouldThrowInsufficientFundsExceptionAndNeverCommit()
    {
        // Arrange
        const string accountNumber = "ACC-20261020-3333";
        var account = new BankAccount(accountNumber, 100m, Guid.CreateVersion7(), "tester", "127.0.0.1");

        _bankAccountRepoMock
            .Setup(r => r.GetByAccountNumberAsync(accountNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var request = new TransactionBankDtoCreate(200m); // Supera los fondos disponibles (100)

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InsufficientFundsException>(() =>
            _service.WithdrawAsync(accountNumber, request, "tester", "127.0.0.1"));

        Assert.Equal(ErrorMessages.InsufficientFunds, exception.Message);

        // El saldo no debe haber cambiado
        Assert.Equal(100m, account.Balance);

        // No se debe persistir la transacción ni confirmar cambios en BD
        _transactionRepoMock.Verify(t => t.AddAsync(It.IsAny<TransactionBank>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetHistoryAsync_WhenAccountExists_ShouldReturnChronologicalTransactions()
    {
        // Arrange
        const string accountNumber = "ACC-20261020-4444";
        var account = new BankAccount(accountNumber, 300m, Guid.CreateVersion7(), "tester", "127.0.0.1");

        _bankAccountRepoMock
            .Setup(r => r.GetByAccountNumberAsync(accountNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var historicalTransactions = new List<TransactionBank>
        {
            new(account.Id, TransactionType.Deposit, 200m, 200m, "tester", "127.0.0.1"),
            new(account.Id, TransactionType.Deposit, 150m, 350m, "tester", "127.0.0.1"),
            new(account.Id, TransactionType.Withdrawal, 50m, 300m, "tester", "127.0.0.1")
        };

        _transactionRepoMock
            .Setup(t => t.GetByBankAccountIdChronologicalAsync(account.Id, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(historicalTransactions);

        // Act
        var result = await _service.GetHistoryAsync(accountNumber);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(200m, result[0].ResultingBalance);
        Assert.Equal(350m, result[1].ResultingBalance);
        Assert.Equal(300m, result[2].ResultingBalance);
    }
}
