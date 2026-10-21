namespace DigitalBank.Domain.Constants;

// Centralizamos todos los mensajes de error y validacion
// para evitar strings hardcodeados
public static class ErrorMessages
{
    public const string InsufficientFunds = "Fondos insuficientes para realizar el retiro.";
    public const string BankAccountNotFound = "Cuenta bancaria especificada no fue encontrada.";
    public const string CustomerNotFound = "Cliente especificado no existe.";
    public const string InvalidAmount = "Monto de la transacción debe ser mayor a cero.";
    public const string NegativeInitialBalance = "Saldo inicial no puede ser un valor negativo.";
    public const string BankAccountNumberAlreadyExists = "Número de cuenta generado ya existe en el sistema.";
    public const string InvalidBirthDate = "Fecha de nacimiento no puede ser futura.";
    public const string InvalidMonthlyIncome = "Ingresos mensuales deben ser mayores o iguales a cero.";
    public const string CustomerNameRequired = "Nombre completo del cliente es obligatorio.";
    public const string AccountNumberRequired = "Número de cuenta bancaria es obligatorio.";
    public const string CustomerRequired = "Identificador del cliente es obligatorio.";
    public const string BankAccountRequired = "Identificador de la cuenta bancaria es obligatorio.";
}
