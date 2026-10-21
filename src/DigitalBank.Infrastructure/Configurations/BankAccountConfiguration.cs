using DigitalBank.Domain.Constants;
using DigitalBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalBank.Infrastructure.Configurations;

public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.ToTable("BankAccounts");

        builder.HasKey(b => b.Id);

        // Requisito  índice único a nivel físico para evitar duplicados
        builder.HasIndex(b => b.AccountNumber)
            .IsUnique();

        builder.Property(b => b.AccountNumber)
            .IsRequired()
            .HasMaxLength(BankConstants.AccountNumberTotalLength);

        builder.Property(b => b.Balance)
            .HasPrecision(18, 2)
            .IsRequired();

        // Relación con Customer (BankAccount -> Customer)
        builder.HasOne(b => b.Customer)
            .WithMany()
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(b => b.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.CreatedIp)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(b => b.UpdatedIp)
            .HasMaxLength(50);
    }
}
