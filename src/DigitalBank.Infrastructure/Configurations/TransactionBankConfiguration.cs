using DigitalBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalBank.Infrastructure.Configurations;

public class TransactionBankConfiguration : IEntityTypeConfiguration<TransactionBank>
{
    public void Configure(EntityTypeBuilder<TransactionBank> builder)
    {
        builder.ToTable("TransactionsBank");

        builder.HasKey(t => t.Id);

        // Índice compuesto para acelerar la consulta cronológica requerida por cuenta
        builder.HasIndex(t => new { t.BankAccountId, t.Timestamp });

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.ResultingBalance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.Timestamp)
            .IsRequired();

        // Relacion con cuenta bancaria 
        builder.HasOne(t => t.BankAccount)
            .WithMany()
            .HasForeignKey(t => t.BankAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(t => t.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.CreatedIp)
            .IsRequired()
            .HasMaxLength(50);
    }
}
