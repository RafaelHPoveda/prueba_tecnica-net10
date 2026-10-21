using DigitalBank.Application.DTOs.TransactionBank;

namespace DigitalBank.Application.Services;

public interface ITransactionBankService
{
    Task<TransactionBankDto> DepositAsync(string accountNumber, TransactionBankDtoCreate request, string user, string ip, CancellationToken cancellationToken = default);
    Task<TransactionBankDto> WithdrawAsync(string accountNumber, TransactionBankDtoCreate request, string user, string ip, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransactionBankDto>> GetHistoryAsync(string accountNumber, CancellationToken cancellationToken = default);
}
