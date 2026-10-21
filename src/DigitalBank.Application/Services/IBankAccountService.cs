using DigitalBank.Application.DTOs.BankAccount;

namespace DigitalBank.Application.Services;

public interface IBankAccountService
{
    Task<BankAccountDto> CreateBankAccountAsync(BankAccountDtoCreate request, string user, string ip, CancellationToken cancellationToken = default);
    Task<BankAccountBalanceDto> GetBalanceAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BankAccountDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BankAccountDto> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task<BankAccountDto> UpdateBankAccountAsync(string accountNumber, BankAccountDtoUpdate request, string user, string ip, CancellationToken cancellationToken = default);
}
