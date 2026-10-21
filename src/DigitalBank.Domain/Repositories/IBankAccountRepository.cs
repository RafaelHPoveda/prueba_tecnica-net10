using DigitalBank.Domain.Entities;

namespace DigitalBank.Domain.Repositories;

public interface IBankAccountRepository : IBaseRepository<BankAccount>
{
    // Buscar por # de cuenta
    Task<BankAccount?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);

    // Vaoidar la existencia de una cuenta por # de cuenta
    Task<bool> ExistsAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);

    // Consulta de todas las cuentas con su cliente asociado
    Task<IReadOnlyList<BankAccount>> GetAllWithCustomerAsync(CancellationToken cancellationToken = default);
}
