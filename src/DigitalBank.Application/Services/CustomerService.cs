using DigitalBank.Application.DTOs.Customer;
using DigitalBank.Domain.Constants;
using DigitalBank.Domain.Entities;
using DigitalBank.Domain.Exceptions;
using DigitalBank.Domain.Repositories;

namespace DigitalBank.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    // Crear cliente
    public async Task<CustomerDto> CreateCustomerAsync(
        CustomerDtoCreate request,
        string user,
        string ip,
        CancellationToken cancellationToken = default)
    {
        var customer = new Customer(
            request.FullName,
            request.BirthDate,
            request.Gender,
            request.MonthlyIncome,
            user,
            ip
        );

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(customer);
    }

    // Actualizar cliente
    public async Task<CustomerDto> UpdateCustomerAsync(
        Guid id,
        CustomerDtoUpdate request,
        string user,
        string ip,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer == null)
        {
            throw new NotFoundException(ErrorMessages.CustomerNotFound);
        }

        customer.Update(
            request.FullName,
            request.BirthDate,
            request.Gender,
            request.MonthlyIncome,
            user,
            ip
        );

        _customerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(customer);
    }

    // Obtener cliente por ID
    public async Task<CustomerDto> GetCustomerByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer == null)
        {
            throw new NotFoundException(ErrorMessages.CustomerNotFound);
        }

        return MapToDto(customer);
    }

    // Obtener todos los clientes
    public async Task<IReadOnlyList<CustomerDto>> GetAllCustomersAsync(
        CancellationToken cancellationToken = default)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);
        return customers.Select(MapToDto).ToList();
    }

    private static CustomerDto MapToDto(Customer customer) => new(
        customer.Id,
        customer.FullName,
        customer.BirthDate,
        customer.Gender,
        customer.MonthlyIncome,
        customer.CreatedAt,
        customer.CreatedBy,
        customer.CreatedIp,
        customer.UpdatedAt,
        customer.UpdatedBy,
        customer.UpdatedIp
    );
}
