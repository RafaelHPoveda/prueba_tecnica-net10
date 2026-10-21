using DigitalBank.Api.Extensions;
using DigitalBank.Application.DTOs.BankAccount;
using DigitalBank.Application.DTOs.TransactionBank;
using DigitalBank.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.Api.Controllers;

[ApiController]
[Route("api/bank-accounts")]
public class BankAccountsController : ControllerBase
{
    private readonly IBankAccountService _bankAccountService;
    private readonly ITransactionBankService _transactionBankService;

    // Desacoplamiento total: solo interactuamos mediante interfaces de aplicación (Application Layer)
    public BankAccountsController(
        IBankAccountService bankAccountService,
        ITransactionBankService transactionBankService)
    {
        _bankAccountService = bankAccountService;
        _transactionBankService = transactionBankService;
    }

    // Obtener todos
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BankAccountDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _bankAccountService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    // Crear Cuenta Bancaria
    [HttpPost]
    public async Task<ActionResult<BankAccountDto>> CreateBankAccount(
        [FromBody] BankAccountDtoCreate request,
        CancellationToken cancellationToken)
    {
        var user = HttpContext.GetCurrentUsername();
        var ip = HttpContext.GetClientIpAddress();

        var result = await _bankAccountService.CreateBankAccountAsync(request, user, ip, cancellationToken);
        return CreatedAtAction(nameof(GetByAccountNumber), new { accountNumber = result.AccountNumber }, result);
    }

    // Consulta de información general de la cuenta
    [HttpGet("{accountNumber}")]
    public async Task<ActionResult<BankAccountDto>> GetByAccountNumber(
        string accountNumber,
        CancellationToken cancellationToken)
    {
        var result = await _bankAccountService.GetByAccountNumberAsync(accountNumber, cancellationToken);
        return Ok(result);
    }

    // Consultar Saldo
    [HttpGet("{accountNumber}/balance")]
    public async Task<ActionResult<BankAccountBalanceDto>> GetBalance(
        string accountNumber,
        CancellationToken cancellationToken)
    {
        var result = await _bankAccountService.GetBalanceAsync(accountNumber, cancellationToken);
        return Ok(result);
    }

    // Actualización de estado de cuenta
    [HttpPut("{accountNumber}")]
    public async Task<ActionResult<BankAccountDto>> UpdateBankAccount(
        string accountNumber,
        [FromBody] BankAccountDtoUpdate request,
        CancellationToken cancellationToken)
    {
        var user = HttpContext.GetCurrentUsername();
        var ip = HttpContext.GetClientIpAddress();

        var result = await _bankAccountService.UpdateBankAccountAsync(accountNumber, request, user, ip, cancellationToken);
        return Ok(result);
    }

    // Registrar Depósitos
    [HttpPost("{accountNumber}/deposits")]
    public async Task<ActionResult<TransactionBankDto>> Deposit(
        string accountNumber,
        [FromBody] TransactionBankDtoCreate request,
        CancellationToken cancellationToken)
    {
        var user = HttpContext.GetCurrentUsername();
        var ip = HttpContext.GetClientIpAddress();

        var result = await _transactionBankService.DepositAsync(accountNumber, request, user, ip, cancellationToken);
        return Ok(result);
    }

    //  Registrar Retiros 
    [HttpPost("{accountNumber}/withdrawals")]
    public async Task<ActionResult<TransactionBankDto>> Withdraw(
        string accountNumber,
        [FromBody] TransactionBankDtoCreate request,
        CancellationToken cancellationToken)
    {
        var user = HttpContext.GetCurrentUsername();
        var ip = HttpContext.GetClientIpAddress();

        var result = await _transactionBankService.WithdrawAsync(accountNumber, request, user, ip, cancellationToken);
        return Ok(result);
    }

    // Historial de Transacciones (cronológico con saldo histórico posterior a cada movimiento)
    [HttpGet("{accountNumber}/transactions")]
    public async Task<ActionResult<IReadOnlyList<TransactionBankDto>>> GetTransactionHistory(
        string accountNumber,
        CancellationToken cancellationToken)
    {
        var result = await _transactionBankService.GetHistoryAsync(accountNumber, cancellationToken);
        return Ok(result);
    }
}
