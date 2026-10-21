namespace DigitalBank.Domain.Exceptions;

// Excepción para cuando un recurso solicitado no existe; se traduce a HTTP 404
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
