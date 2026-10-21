namespace DigitalBank.Application.DTOs.TransactionBank;

public record TransactionBankDto(
    Guid TransactionId,
    string Type,
    decimal Amount,
    DateTime Timestamp,
    decimal ResultingBalance,
    string CreatedBy,
    string CreatedIp
);
