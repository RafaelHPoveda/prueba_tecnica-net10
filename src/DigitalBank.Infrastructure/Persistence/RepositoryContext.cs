using DigitalBank.Domain.Entities;
using DigitalBank.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.Infrastructure.Persistence;

public class RepositoryContext : DbContext
{
    public RepositoryContext(DbContextOptions<RepositoryContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<TransactionBank> TransactionsBank { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new BankAccountConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionBankConfiguration());
    }
}
