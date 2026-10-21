using DigitalBank.Application.Common.Interfaces;
using DigitalBank.Application.DTOs.BankAccount;
using DigitalBank.Domain.Constants;
using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Enums;
using DigitalBank.Domain.Exceptions;
using DigitalBank.Domain.Repositories;

namespace DigitalBank.Application.Services;

public class BankAccountService : IBankAccountService
{
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ITransactionBankRepository _transactionBankRepository;
    private readonly IAccountNumberGenerator _accountNumberGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public BankAccountService(
        IBankAccountRepository bankAccountRepository,
        ICustomerRepository customerRepository,
        ITransactionBankRepository transactionBankRepository,
        IAccountNumberGenerator accountNumberGenerator,
        IUnitOfWork unitOfWork)
    {
        _bankAccountRepository = bankAccountRepository;
        _customerRepository = customerRepository;
        _transactionBankRepository = transactionBankRepository;
        _accountNumberGenerator = accountNumberGenerator;
        _unitOfWork = unitOfWork;
    }

    // Crear cliente
    public async Task<BankAccountDto> CreateBankAccountAsync(
        BankAccountDtoCreate request,
        string user,
        string ip,
        CancellationToken cancellationToken = default)
    {
        // Verificar que el cliente exista
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new NotFoundException(ErrorMessages.CustomerNotFound);
        }

        // Generar un número de cuenta único con reintentos para evitar colisiones
        string accountNumber;
        int attempts = 0;
        do
        {
            accountNumber = _accountNumberGenerator.Generate();
            attempts++;
            if (attempts > 10)
            {
                throw new BusinessException(ErrorMessages.BankAccountNumberAlreadyExists);
            }
        } while (await _bankAccountRepository.ExistsAccountNumberAsync(accountNumber, cancellationToken));

        // Instanciamos la cuenta bancaria (el modelo valida el saldo inicial >= 0)
        var account = new BankAccount(accountNumber, request.InitialBalance, request.CustomerId, user, ip);
        await _bankAccountRepository.AddAsync(account, cancellationToken);

        // Si la cuenta nace con saldo inicial, registramos el movimiento de apertura
        if (request.InitialBalance > 0)
        {
            var initialTransaction = new TransactionBank(
                account.Id,
                TransactionType.Deposit,
                request.InitialBalance,
                request.InitialBalance,
                user,
                ip
            );
            await _transactionBankRepository.AddAsync(initialTransaction, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(account, customer.FullName);
    }

    // Obtener todas las cuentas
    public async Task<IReadOnlyList<BankAccountDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var accounts = await _bankAccountRepository.GetAllWithCustomerAsync(cancellationToken);

        var count = accounts.Count;
        var result = new List<BankAccountDto>(count);
        for (int i = 0; i < count; i++)
        {
            var acc = accounts[i];
            result.Add(MapToDto(acc, acc.Customer?.FullName));
        }

        return result;
    }

    // Obtener saldos de cuentas
    public async Task<BankAccountBalanceDto> GetBalanceAsync(
        string accountNumber,
        CancellationToken cancellationToken = default)
    {
        var account = await _bankAccountRepository.GetByAccountNumberAsync(accountNumber, cancellationToken);
        if (account == null)
        {
            throw new NotFoundException(ErrorMessages.BankAccountNotFound);
        }

        return new BankAccountBalanceDto(account.AccountNumber, account.Balance);
    }

    // Obtener cuenta por #
    public async Task<BankAccountDto> GetByAccountNumberAsync(
        string accountNumber,
        CancellationToken cancellationToken = default)
    {
        var account = await _bankAccountRepository.GetByAccountNumberAsync(accountNumber, cancellationToken);
        if (account == null)
        {
            throw new NotFoundException(ErrorMessages.BankAccountNotFound);
        }

        return MapToDto(account, account.Customer?.FullName);
    }

    // Actualizar cuenta
    public async Task<BankAccountDto> UpdateBankAccountAsync(
        string accountNumber,
        BankAccountDtoUpdate request,
        string user,
        string ip,
        CancellationToken cancellationToken = default)
    {
        var account = await _bankAccountRepository.GetByAccountNumberAsync(accountNumber, cancellationToken);
        if (account == null)
        {
            throw new NotFoundException(ErrorMessages.BankAccountNotFound);
        }

        // Registramos auditoría de la actualización
        account.SetUpdateAudit(user, ip);
        _bankAccountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(account, account.Customer?.FullName);
    }

    private static BankAccountDto MapToDto(BankAccount account, string? customerName) => new(
        account.Id,
        account.AccountNumber,
        account.Balance,
        account.CustomerId,
        customerName,
        account.CreatedAt,
        account.CreatedBy,
        account.CreatedIp,
        account.UpdatedAt,
        account.UpdatedBy,
        account.UpdatedIp
    );
}
