using DigitalBank.Domain.Constants;

namespace DigitalBank.Domain.Exceptions;

// Excepción puntual para rechazar un retiro cuando no alcanza el saldo
public class InsufficientFundsException : BusinessException
{
    public InsufficientFundsException(string message = ErrorMessages.InsufficientFunds) 
        : base(message)
    {
    }
}
