using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DigitalBank.Domain.Common;
using DigitalBank.Domain.Constants;
using DigitalBank.Domain.Enums;
using DigitalBank.Domain.Exceptions;

namespace DigitalBank.Domain.Entities;

// Tabla de clientes
[Table("Customers")]
public class Customer : AuditableEntity
{
    [Required(ErrorMessage = ErrorMessages.CustomerNameRequired)]
    [MaxLength(150)]
    public string FullName { get; private set; } = string.Empty;

    [Required]
    public DateOnly BirthDate { get; private set; }

    [Required]
    public Gender Gender { get; private set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, (double)decimal.MaxValue, ErrorMessage = ErrorMessages.InvalidMonthlyIncome)]
    public decimal MonthlyIncome { get; private set; }

    protected Customer() { }

    // sobrecarga
    public Customer(
        string fullName,
        DateOnly birthDate,
        Gender gender,
        decimal monthlyIncome,
        string createdBy,
        string createdIp)
    {
        ValidateCustomerData(fullName, birthDate, monthlyIncome);

        FullName = fullName.Trim();
        BirthDate = birthDate;
        Gender = gender;
        MonthlyIncome = monthlyIncome;

        SetCreationAudit(createdBy, createdIp);
    }

    // Actualizar datos del cliente
    public void Update(
        string fullName,
        DateOnly birthDate,
        Gender gender,
        decimal monthlyIncome,
        string updatedBy,
        string updatedIp)
    {
        ValidateCustomerData(fullName, birthDate, monthlyIncome);

        FullName = fullName.Trim();
        BirthDate = birthDate;
        Gender = gender;
        MonthlyIncome = monthlyIncome;

        SetUpdateAudit(updatedBy, updatedIp);
    }

    // Validar datos del cliente
    private static void ValidateCustomerData(string fullName, DateOnly birthDate, decimal monthlyIncome)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new BusinessException(ErrorMessages.CustomerNameRequired);
        }

        if (birthDate > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new BusinessException(ErrorMessages.InvalidBirthDate);
        }

        if (monthlyIncome < 0)
        {
            throw new BusinessException(ErrorMessages.InvalidMonthlyIncome);
        }
    }
}
