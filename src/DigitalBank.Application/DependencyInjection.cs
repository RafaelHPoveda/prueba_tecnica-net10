using DigitalBank.Application.Common.Interfaces;
using DigitalBank.Application.Common.Services;
using DigitalBank.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalBank.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Generador de números de cuenta (formato ACC-YYYYMMDD-XXXX)
        services.AddSingleton<IAccountNumberGenerator, AccountNumberGenerator>();

        // Servicios de aplicación (casos de uso)
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IBankAccountService, BankAccountService>();
        services.AddScoped<ITransactionBankService, TransactionBankService>();

        return services;
    }
}
