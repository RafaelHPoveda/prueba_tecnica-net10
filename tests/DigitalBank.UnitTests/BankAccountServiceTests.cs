using DigitalBank.Application.Common.Interfaces;
using DigitalBank.Application.DTOs.BankAccount;
using DigitalBank.Application.Services;
using DigitalBank.Domain.Constants;
using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Enums;
using DigitalBank.Domain.Exceptions;
using DigitalBank.Domain.Repositories;
using Moq;

namespace DigitalBank.UnitTests;

public class BankAccountServiceTests
{
    private readonly Mock<IBankAccountRepository> _bankAccountRepoMock = new();
    private readonly Mock<ICustomerRepository> _customerRepoMock = new();
    private readonly Mock<ITransactionBankRepository> _transactionRepoMock = new();
    private readonly Mock<IAccountNumberGenerator> _generatorMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly BankAccountService _service;

    public BankAccountServiceTests()
    {
        _service = new BankAccountService(
            _bankAccountRepoMock.Object,
            _customerRepoMock.Object,
            _transactionRepoMock.Object,
            _generatorMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task CreateBankAccountAsync_WhenCustomerExistsAndInitialBalancePositive_ShouldCreateAccountAndInitialDeposit()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var customer = new Customer("Carlos Santana", new DateOnly(1985, 4, 12), Gender.Male, 4000m, "admin", "127.0.0.1");

        _customerRepoMock
            .Setup(c => c.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        const string generatedNumber = "ACC-20261020-5555";
        _generatorMock.Setup(g => g.Generate()).Returns(generatedNumber);

        _bankAccountRepoMock
            .Setup(b => b.ExistsAccountNumberAsync(generatedNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new BankAccountDtoCreate(customerId, 500m);

        // Act
        var result = await _service.CreateBankAccountAsync(request, "admin", "127.0.0.1");

        // Assert
        Assert.Equal(generatedNumber, result.AccountNumber);
        Assert.Equal(500m, result.Balance);
        Assert.Equal(customerId, result.CustomerId);

        // Verifica que la cuenta bancaria se agregó
        _bankAccountRepoMock.Verify(b => b.AddAsync(It.Is<BankAccount>(acc =>
            acc.AccountNumber == generatedNumber && acc.Balance == 500m), It.IsAny<CancellationToken>()), Times.Once);

        // Verifica que se creó el movimiento inicial de depósito
        _transactionRepoMock.Verify(t => t.AddAsync(It.Is<TransactionBank>(tb =>
            tb.Amount == 500m && tb.Type == TransactionType.Deposit), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBankAccountAsync_WhenCustomerDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var nonExistentCustomer = Guid.CreateVersion7();
        _customerRepoMock
            .Setup(c => c.GetByIdAsync(nonExistentCustomer, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var request = new BankAccountDtoCreate(nonExistentCustomer, 100m);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.CreateBankAccountAsync(request, "admin", "127.0.0.1"));

        Assert.Equal(ErrorMessages.CustomerNotFound, exception.Message);
        _bankAccountRepoMock.Verify(b => b.AddAsync(It.IsAny<BankAccount>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetBalanceAsync_WhenAccountExists_ShouldReturnBalance()
    {
        // Arrange
        const string accountNumber = "ACC-20261020-7777";
        var account = new BankAccount(accountNumber, 850m, Guid.CreateVersion7(), "admin", "127.0.0.1");

        _bankAccountRepoMock
            .Setup(b => b.GetByAccountNumberAsync(accountNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        var result = await _service.GetBalanceAsync(accountNumber);

        // Assert
        Assert.Equal(accountNumber, result.AccountNumber);
        Assert.Equal(850m, result.CurrentBalance);
    }

    [Fact]
    public async Task GetBalanceAsync_WhenAccountDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        const string nonExistent = "ACC-20261020-0000";
        _bankAccountRepoMock
            .Setup(b => b.GetByAccountNumberAsync(nonExistent, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BankAccount?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.GetBalanceAsync(nonExistent));

        Assert.Equal(ErrorMessages.BankAccountNotFound, exception.Message);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllAccountsWithCustomerName()
    {
        // Arrange
        var customer = new Customer("Lucia Méndez", new DateOnly(1992, 1, 1), Gender.Female, 2000m, "admin", "127.0.0.1");
        var accounts = new List<BankAccount>
        {
            new("ACC-20261020-0001", 100m, customer.Id, "admin", "127.0.0.1") { Customer = customer },
            new("ACC-20261020-0002", 200m, customer.Id, "admin", "127.0.0.1") { Customer = customer }
        };

        _bankAccountRepoMock
            .Setup(b => b.GetAllWithCustomerAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("ACC-20261020-0001", result[0].AccountNumber);
        Assert.Equal("Lucia Méndez", result[0].CustomerName);
        Assert.Equal("ACC-20261020-0002", result[1].AccountNumber);
    }
}
