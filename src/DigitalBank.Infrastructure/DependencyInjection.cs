using DigitalBank.Domain.Repositories;
using DigitalBank.Infrastructure.Persistence;
using DigitalBank.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalBank.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=digitalbank.db";

        services.AddDbContext<RepositoryContext>(options =>
            options.UseSqlite(connectionString));

        // Registro de repositorios
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IBankAccountRepository, BankAccountRepository>();
        services.AddScoped<ITransactionBankRepository, TransactionBankRepository>();

        // Registro de Unit of Work para consistencia atómica
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
