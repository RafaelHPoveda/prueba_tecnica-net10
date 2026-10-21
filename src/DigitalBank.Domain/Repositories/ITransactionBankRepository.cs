using DigitalBank.Domain.Entities;

namespace DigitalBank.Domain.Repositories;

public interface ITransactionBankRepository : IBaseRepository<TransactionBank>
{
    // Obtiene el historial ordenado cronolo'gicamente para una cuenta bancaria
    Task<IReadOnlyList<TransactionBank>> GetByBankAccountIdChronologicalAsync(
        Guid bankAccountId,
        bool ascending = true,
        CancellationToken cancellationToken = default);
}
