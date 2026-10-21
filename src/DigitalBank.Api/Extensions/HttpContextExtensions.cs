using DigitalBank.Domain.Constants;

namespace DigitalBank.Api.Extensions;

public static class HttpContextExtensions
{
    // Resuelve la dirección IP del cliente tomando en cuenta posibles proxies inversos
    public static string GetClientIpAddress(this HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor) &&
            !string.IsNullOrWhiteSpace(forwardedFor))
        {
            var firstIp = forwardedFor.ToString().Split(',')[0].Trim();
            if (!string.IsNullOrWhiteSpace(firstIp))
            {
                return firstIp;
            }
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? BankConstants.DefaultLocalIp;
    }

    public static string GetCurrentUsername(this HttpContext context)
    {
        return context.User.Identity?.Name ?? BankConstants.DefaultSystemUser;
    }
}
