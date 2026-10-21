namespace DigitalBank.Application.Common.Interfaces;

public interface IAccountNumberGenerator
{
    // Genera un número de cuenta bajo el formato estricto: ACC-YYYYMMDD-[XXXX]
    string Generate();
}
