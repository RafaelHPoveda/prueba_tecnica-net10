namespace DigitalBank.Application.DTOs.BankAccount;

public record BankAccountDto(
    Guid Id,
    string AccountNumber,
    decimal Balance,
    Guid CustomerId,
    string? CustomerName,
    DateTime CreatedAt,
    string CreatedBy,
    string CreatedIp,
    DateTime? UpdatedAt,
    string? UpdatedBy,
    string? UpdatedIp
);
