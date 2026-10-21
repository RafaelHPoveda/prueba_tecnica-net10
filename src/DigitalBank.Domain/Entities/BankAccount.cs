using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalBank.Domain.Common;
using DigitalBank.Domain.Constants;
using DigitalBank.Domain.Exceptions;

namespace DigitalBank.Domain.Entities;

// Modelo de las Cuentas Bancarias
[Table("BankAccounts")]
public class BankAccount : AuditableEntity
{
    [Required]
    [MaxLength(BankConstants.AccountNumberTotalLength)]
    public string AccountNumber { get; private set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, (double)decimal.MaxValue, ErrorMessage = ErrorMessages.NegativeInitialBalance)]
    public decimal Balance { get; private set; }

    [Required]
    [ForeignKey(nameof(Customer))]
    public Guid CustomerId { get; private set; }

    // Navegacion hacia cliente
    public virtual Customer? Customer { get; set; }

    protected BankAccount() { }

    // sobrecarga
    public BankAccount(
        string accountNumber,
        decimal initialBalance,
        Guid customerId,
        string createdBy,
        string createdIp)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new BusinessException(ErrorMessages.AccountNumberRequired);
        }

        if (initialBalance < 0)
        {
            throw new BusinessException(ErrorMessages.NegativeInitialBalance);
        }

        AccountNumber = accountNumber.Trim();
        Balance = initialBalance;
        CustomerId = customerId;

        SetCreationAudit(createdBy, createdIp);
    }

    // Validar monto positivo y actualizar el saldo de la cuenta bancaria
    public decimal Deposit(decimal amount, string user, string ip)
    {
        if (amount <= 0)
        {
            throw new BusinessException(ErrorMessages.InvalidAmount);
        }

        Balance += amount;
        SetUpdateAudit(user, ip);
        return Balance;
    }

    // Validar monto positivo y verificar fondos suficientes antes de actualizar el saldo de la cuenta bancaria
    public decimal Withdraw(decimal amount, string user, string ip)
    {
        if (amount <= 0)
        {
            throw new BusinessException(ErrorMessages.InvalidAmount);
        }

        if (Balance < amount)
        {
            throw new InsufficientFundsException();
        }

        Balance -= amount;
        SetUpdateAudit(user, ip);
        return Balance;
    }
}
