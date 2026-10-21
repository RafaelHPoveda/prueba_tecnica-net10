using DigitalBank.Domain.Constants;
using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Exceptions;

namespace DigitalBank.UnitTests;

// Pruebas unitarias de las reglas de negocio del modelo (Rich Domain Model)
public class BankAccountDomainTests
{
    private const string TestAccountNumber = "ACC-20261020-1234";
    private readonly Guid _customerId = Guid.CreateVersion7();

    [Fact]
    public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var account = new BankAccount(TestAccountNumber, 500m, _customerId, "admin", "127.0.0.1");

        // Assert
        Assert.Equal(TestAccountNumber, account.AccountNumber);
        Assert.Equal(500m, account.Balance);
        Assert.Equal(_customerId, account.CustomerId);
    }

    [Fact]
    public void Constructor_WithNegativeInitialBalance_ShouldThrowBusinessException()
    {
        // Act & Assert
        var exception = Assert.Throws<BusinessException>(() =>
            new BankAccount(TestAccountNumber, -50m, _customerId, "admin", "127.0.0.1"));

        Assert.Equal(ErrorMessages.NegativeInitialBalance, exception.Message);
    }

    [Fact]
    public void Deposit_WithPositiveAmount_ShouldIncreaseBalance()
    {
        // Arrange
        var account = new BankAccount(TestAccountNumber, 100m, _customerId, "admin", "127.0.0.1");

        // Act
        var resultingBalance = account.Deposit(150m, "admin", "127.0.0.1");

        // Assert
        Assert.Equal(250m, account.Balance);
        Assert.Equal(250m, resultingBalance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Deposit_WithZeroOrNegativeAmount_ShouldThrowBusinessException(decimal invalidAmount)
    {
        // Arrange
        var account = new BankAccount(TestAccountNumber, 100m, _customerId, "admin", "127.0.0.1");

        // Act & Assert
        var exception = Assert.Throws<BusinessException>(() =>
            account.Deposit(invalidAmount, "admin", "127.0.0.1"));

        Assert.Equal(ErrorMessages.InvalidAmount, exception.Message);
        Assert.Equal(100m, account.Balance); // El saldo debe mantenerse intacto
    }

    [Fact]
    public void Withdraw_WithSufficientFunds_ShouldDecreaseBalance()
    {
        // Arrange
        var account = new BankAccount(TestAccountNumber, 300m, _customerId, "admin", "127.0.0.1");

        // Act
        var resultingBalance = account.Withdraw(100m, "admin", "127.0.0.1");

        // Assert
        Assert.Equal(200m, account.Balance);
        Assert.Equal(200m, resultingBalance);
    }

    [Fact]
    public void Withdraw_WithExactBalance_ShouldAllowWithdrawalAndLeaveZeroBalance()
    {
        // Arrange
        var account = new BankAccount(TestAccountNumber, 250m, _customerId, "admin", "127.0.0.1");

        // Act
        var resultingBalance = account.Withdraw(250m, "admin", "127.0.0.1");

        // Assert
        Assert.Equal(0m, account.Balance);
        Assert.Equal(0m, resultingBalance);
    }

    [Fact]
    public void Withdraw_WithInsufficientFunds_ShouldThrowInsufficientFundsException()
    {
        // Arrange
        var account = new BankAccount(TestAccountNumber, 100m, _customerId, "admin", "127.0.0.1");

        // Act & Assert
        var exception = Assert.Throws<InsufficientFundsException>(() =>
            account.Withdraw(150m, "admin", "127.0.0.1"));

        Assert.Equal(ErrorMessages.InsufficientFunds, exception.Message);
        Assert.Equal(100m, account.Balance); // Verificamos que el saldo no se alteró
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-25)]
    public void Withdraw_WithZeroOrNegativeAmount_ShouldThrowBusinessException(decimal invalidAmount)
    {
        // Arrange
        var account = new BankAccount(TestAccountNumber, 100m, _customerId, "admin", "127.0.0.1");

        // Act & Assert
        var exception = Assert.Throws<BusinessException>(() =>
            account.Withdraw(invalidAmount, "admin", "127.0.0.1"));

        Assert.Equal(ErrorMessages.InvalidAmount, exception.Message);
        Assert.Equal(100m, account.Balance);
    }
}
