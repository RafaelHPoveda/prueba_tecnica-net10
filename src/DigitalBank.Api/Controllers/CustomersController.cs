using DigitalBank.Api.Extensions;
using DigitalBank.Application.DTOs.Customer;
using DigitalBank.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    // Desacoplamiento estricto: el controlador solo conoce la interfaz de aplicación
    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(
        [FromBody] CustomerDtoCreate request, 
        CancellationToken cancellationToken)
    {
        var user = HttpContext.GetCurrentUsername();
        var ip = HttpContext.GetClientIpAddress();

        var result = await _customerService.CreateCustomerAsync(request, user, ip, cancellationToken);
        return CreatedAtAction(nameof(GetCustomerById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(
        Guid id, 
        CancellationToken cancellationToken)
    {
        var result = await _customerService.GetCustomerByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetAllCustomers(
        CancellationToken cancellationToken)
    {
        var result = await _customerService.GetAllCustomersAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> UpdateCustomer(
        Guid id, 
        [FromBody] CustomerDtoUpdate request, 
        CancellationToken cancellationToken)
    {
        var user = HttpContext.GetCurrentUsername();
        var ip = HttpContext.GetClientIpAddress();

        var result = await _customerService.UpdateCustomerAsync(id, request, user, ip, cancellationToken);
        return Ok(result);
    }
}
