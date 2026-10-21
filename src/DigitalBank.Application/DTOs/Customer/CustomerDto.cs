using DigitalBank.Domain.Enums;

namespace DigitalBank.Application.DTOs.Customer;

public record CustomerDto(
    Guid Id,
    string FullName,
    DateOnly BirthDate,
    Gender Gender,
    decimal MonthlyIncome,
    DateTime CreatedAt,
    string CreatedBy,
    string CreatedIp,
    DateTime? UpdatedAt,
    string? UpdatedBy,
    string? UpdatedIp
);
