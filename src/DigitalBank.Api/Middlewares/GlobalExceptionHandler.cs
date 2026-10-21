using DigitalBank.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBank.Api.Middlewares;

// Manejador global de excepciones para ASP.NET Core (.NET 10).
// Atrapa los errores de negocio y los traduce a respuestas HTTP limpias (400, 404, 500)
// sin necesidad de llenar los controladores de bloques try-catch repetitivos.
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        _logger.LogWarning(exception, "Excepción capturada por el GlobalExceptionHandler: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                "Recurso no encontrado",
                notFoundEx.Message
            ),
            InsufficientFundsException fundsEx => (
                StatusCodes.Status400BadRequest,
                "Fondos insuficientes",
                fundsEx.Message
            ),
            BusinessException businessEx => (
                StatusCodes.Status400BadRequest,
                "Operación bancaria inválida",
                businessEx.Message
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Error interno del servidor",
                "Ocurrió un error inesperado al procesar la transacción bancaria. Por favor intente más tarde."
            )
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
