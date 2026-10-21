using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Repositories;
using DigitalBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.Infrastructure.Repositories;

public class BankAccountRepository : BaseRepository<BankAccount>, IBankAccountRepository
{
    public BankAccountRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<BankAccount?> GetByAccountNumberAsync(
        string accountNumber,
        CancellationToken cancellationToken = default)
    {
        // Incluimos información del cliente
        return await DbSet
            .Include(b => b.Customer)
            .FirstOrDefaultAsync(b => b.AccountNumber == accountNumber, cancellationToken);
    }

    public async Task<bool> ExistsAccountNumberAsync(
        string accountNumber,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(b => b.AccountNumber == accountNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<BankAccount>> GetAllWithCustomerAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(b => b.Customer)
            .ToListAsync(cancellationToken);
    }
}
