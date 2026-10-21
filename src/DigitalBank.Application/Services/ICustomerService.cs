using DigitalBank.Application.DTOs.Customer;

namespace DigitalBank.Application.Services;

public interface ICustomerService
{
    Task<CustomerDto> CreateCustomerAsync(CustomerDtoCreate request, string user, string ip, CancellationToken cancellationToken = default);
    Task<CustomerDto> UpdateCustomerAsync(Guid id, CustomerDtoUpdate request, string user, string ip, CancellationToken cancellationToken = default);
    Task<CustomerDto> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustomerDto>> GetAllCustomersAsync(CancellationToken cancellationToken = default);
}
