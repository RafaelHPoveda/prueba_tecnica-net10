using System.ComponentModel.DataAnnotations;
using DigitalBank.Domain.Constants;
using DigitalBank.Domain.Enums;

namespace DigitalBank.Application.DTOs.Customer;

public record CustomerDtoUpdate(
    [Required(ErrorMessage = ErrorMessages.CustomerNameRequired)]
    [MaxLength(150)]
    string FullName,

    [Required]
    DateOnly BirthDate,

    [Required]
    Gender Gender,

    [Required]
    [Range(0, (double)decimal.MaxValue, ErrorMessage = ErrorMessages.InvalidMonthlyIncome)]
    decimal MonthlyIncome
);
