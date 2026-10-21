using DigitalBank.Application.DTOs.TransactionBank;
using DigitalBank.Domain.Constants;
using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Enums;
using DigitalBank.Domain.Exceptions;
using DigitalBank.Domain.Repositories;

namespace DigitalBank.Application.Services;

public class TransactionBankService : ITransactionBankService
{
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly ITransactionBankRepository _transactionBankRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBankService(
        IBankAccountRepository bankAccountRepository,
        ITransactionBankRepository transactionBankRepository,
        IUnitOfWork unitOfWork)
    {
        _bankAccountRepository = bankAccountRepository;
        _transactionBankRepository = transactionBankRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TransactionBankDto> DepositAsync(
        string accountNumber,
        TransactionBankDtoCreate request,
        string user,
        string ip,
        CancellationToken cancellationToken = default)
    {
        var account = await _bankAccountRepository.GetByAccountNumberAsync(accountNumber, cancellationToken);
        if (account == null)
        {
            throw new NotFoundException(ErrorMessages.BankAccountNotFound);
        }

        // El modelo valida que el monto sea positivo e incrementa el balance
        var resultingBalance = account.Deposit(request.Amount, user, ip);

        var transaction = new TransactionBank(
            account.Id,
            TransactionType.Deposit,
            request.Amount,
            resultingBalance,
            user,
            ip
        );

        _bankAccountRepository.Update(account);
        await _transactionBankRepository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(transaction);
    }

    public async Task<TransactionBankDto> WithdrawAsync(
        string accountNumber,
        TransactionBankDtoCreate request,
        string user,
        string ip,
        CancellationToken cancellationToken = default)
    {
        var account = await _bankAccountRepository.GetByAccountNumberAsync(accountNumber, cancellationToken);
        if (account == null)
        {
            throw new NotFoundException(ErrorMessages.BankAccountNotFound);
        }

        // Si los fondos son insuficientes, el método Withdraw lanza InsufficientFundsException limpiamente
        var resultingBalance = account.Withdraw(request.Amount, user, ip);

        var transaction = new TransactionBank(
            account.Id,
            TransactionType.Withdrawal,
            request.Amount,
            resultingBalance,
            user,
            ip
        );

        _bankAccountRepository.Update(account);
        await _transactionBankRepository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(transaction);
    }

    public async Task<IReadOnlyList<TransactionBankDto>> GetHistoryAsync(
        string accountNumber,
        CancellationToken cancellationToken = default)
    {
        var account = await _bankAccountRepository.GetByAccountNumberAsync(accountNumber, cancellationToken);
        if (account == null)
        {
            throw new NotFoundException(ErrorMessages.BankAccountNotFound);
        }

        var transactions = await _transactionBankRepository.GetByBankAccountIdChronologicalAsync(
            account.Id,
            ascending: true,
            cancellationToken
        );

        return transactions.Select(MapToDto).ToList();
    }

    private static TransactionBankDto MapToDto(TransactionBank transaction) => new(
        transaction.Id,
        transaction.Type.ToString(),
        transaction.Amount,
        transaction.Timestamp,
        transaction.ResultingBalance,
        transaction.CreatedBy,
        transaction.CreatedIp
    );
}
