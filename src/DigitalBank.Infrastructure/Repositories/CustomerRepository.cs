using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Repositories;
using DigitalBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.Infrastructure.Repositories;

public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(c => c.Id == id, cancellationToken);
    }
}
