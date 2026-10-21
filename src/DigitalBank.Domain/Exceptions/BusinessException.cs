namespace DigitalBank.Domain.Exceptions;

// Excepción base para reglas del negocio; el manejador global de errores la traduce a HTTP 400
public class BusinessException : Exception
{
    public BusinessException(string message) : base(message)
    {
    }

    public BusinessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
