using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalBank.Domain.Common;
using DigitalBank.Domain.Constants;
using DigitalBank.Domain.Enums;
using DigitalBank.Domain.Exceptions;

namespace DigitalBank.Domain.Entities;

// Tabla de transacciones bancarias
[Table("TransactionsBank")]
public class TransactionBank : AuditableEntity
{
    [Required]
    [ForeignKey(nameof(BankAccount))]
    public Guid BankAccountId { get; private set; }

    public BankAccount? BankAccount { get; set; }

    [Required]
    public TransactionType Type { get; private set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = ErrorMessages.InvalidAmount)]
    public decimal Amount { get; private set; }

    [Required]
    public DateTime Timestamp { get; private set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ResultingBalance { get; private set; }

    protected TransactionBank() { }

    public TransactionBank(
        Guid bankAccountId,
        TransactionType type,
        decimal amount,
        decimal resultingBalance,
        string createdBy,
        string createdIp)
    {
        if (amount <= 0)
        {
            throw new BusinessException(ErrorMessages.InvalidAmount);
        }

        BankAccountId = bankAccountId;
        Type = type;
        Amount = amount;
        Timestamp = DateTime.UtcNow;
        ResultingBalance = resultingBalance;

        SetCreationAudit(createdBy, createdIp);
    }
}
