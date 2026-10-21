using System.Text.RegularExpressions;
using DigitalBank.Application.Common.Services;
using DigitalBank.Domain.Constants;

namespace DigitalBank.UnitTests;

// Pruebas requeridas por el enunciado para validar la integridad del generador de cuentas:
// - Longitud correcta (17 caracteres)
// - Prefijo correcto ("ACC-")
// - Fecha de hoy (YYYYMMDD)
// - Unicidad básica (sin colisiones en ejecuciones consecutivas)
public class AccountNumberGeneratorTests
{
    private readonly AccountNumberGenerator _generator = new();

    [Fact]
    public void Generate_ShouldStartWithCorrectPrefix()
    {
        // Act
        var accountNumber = _generator.Generate();

        // Assert
        Assert.StartsWith(BankConstants.AccountPrefix, accountNumber);
    }

    [Fact]
    public void Generate_ShouldHaveExactLengthOfSeventeenCharacters()
    {
        // Act
        var accountNumber = _generator.Generate();

        // Assert
        // ACC- (4) + YYYYMMDD (8) + - (1) + XXXX (4) = 17 caracteres
        Assert.Equal(BankConstants.AccountNumberTotalLength, accountNumber.Length);
    }

    [Fact]
    public void Generate_ShouldContainCurrentDateInUtc()
    {
        // Arrange
        var todayPart = DateTime.UtcNow.ToString("yyyyMMdd");

        // Act
        var accountNumber = _generator.Generate();

        // Assert
        // Validamos que la porción de fecha en el número de cuenta coincida con el día actual
        var dateInAccountNumber = accountNumber.Substring(4, 8);
        Assert.Equal(todayPart, dateInAccountNumber);
    }

    [Fact]
    public void Generate_ShouldMatchExactFormatWithRegularExpression()
    {
        // Act
        var accountNumber = _generator.Generate();

        // Assert
        // Formato exacto: ACC-YYYYMMDD-XXXX
        var pattern = @"^ACC-\d{8}-\d{4}$";
        Assert.Matches(pattern, accountNumber);
    }

    [Fact]
    public void Generate_ConsecutiveCalls_ShouldProvideBasicUniqueness()
    {
        // Act
        var first = _generator.Generate();
        var second = _generator.Generate();
        var third = _generator.Generate();

        // Assert: Unicidadz
        Assert.NotEqual(first, second);
        Assert.NotEqual(second, third);
    }
}
