using DigitalBank.Domain.Entities;

namespace DigitalBank.Domain.Repositories;

public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
