using System.Security.Cryptography;
using DigitalBank.Application.Common.Interfaces;
using DigitalBank.Domain.Constants;

namespace DigitalBank.Application.Common.Services;

public class AccountNumberGenerator : IAccountNumberGenerator
{
    // Algoritmo requerido
    // Formato exacto: ACC-YYYYMMDD-XXXX
    // Prefijo constante: ACC-
    // YYYYMMDD: año, mes y día actual
    // XXXX: 4 dígitos aleatorios 
    public string Generate()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomDigits = RandomNumberGenerator.GetInt32(0, 10000).ToString("D4");

        return $"{BankConstants.AccountPrefix}{datePart}-{randomDigits}";
    }
}
