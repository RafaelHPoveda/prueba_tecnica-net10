using System.ComponentModel.DataAnnotations;
using DigitalBank.Domain.Constants;

namespace DigitalBank.Application.DTOs.BankAccount;

public record BankAccountDtoCreate(
    [Required(ErrorMessage = ErrorMessages.CustomerRequired)]
    Guid CustomerId,

    [Required]
    [Range(0, (double)decimal.MaxValue, ErrorMessage = ErrorMessages.NegativeInitialBalance)]
    decimal InitialBalance
);
