using System.ComponentModel.DataAnnotations;
using DigitalBank.Domain.Constants;

namespace DigitalBank.Application.DTOs.TransactionBank;

public record TransactionBankDtoCreate(
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = ErrorMessages.InvalidAmount)]
    decimal Amount
);
