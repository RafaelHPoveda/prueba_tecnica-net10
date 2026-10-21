using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Repositories;
using DigitalBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.Infrastructure.Repositories;

public class TransactionBankRepository : BaseRepository<TransactionBank>, ITransactionBankRepository
{
    public TransactionBankRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<IReadOnlyList<TransactionBank>> GetByBankAccountIdChronologicalAsync(
        Guid bankAccountId,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(t => t.BankAccountId == bankAccountId);

        query = ascending
            ? query.OrderBy(t => t.Timestamp)
            : query.OrderByDescending(t => t.Timestamp);

        return await query.ToListAsync(cancellationToken);
    }
}
