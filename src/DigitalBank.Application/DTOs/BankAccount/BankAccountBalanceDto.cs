namespace DigitalBank.Application.DTOs.BankAccount;

public record BankAccountBalanceDto(
    string AccountNumber,
    decimal CurrentBalance
);
