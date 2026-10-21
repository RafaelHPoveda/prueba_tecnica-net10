namespace DigitalBank.Application.DTOs.BankAccount;

// DTO para soportar futuras actualizaciones administrativas de la cuenta
public record BankAccountDtoUpdate(
    bool IsActive = true
);
