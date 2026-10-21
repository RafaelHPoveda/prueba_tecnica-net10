namespace DigitalBank.Domain.Constants;

public static class BankConstants
{
    // Prefijo exigido por la prueba para generar las cuentas bancarias
    public const string AccountPrefix = "ACC-";

    // Longitud exacta esperada: ACC-(4) + YYYYMMDD(8) + -(1) + XXXX(4) = 17 caracteres
    public const int AccountNumberTotalLength = 17;

    // Valor por defecto para los campos de auditoria
    public const string DefaultSystemUser = "super.user";
    public const string DefaultLocalIp = "127.0.0.1";
}
