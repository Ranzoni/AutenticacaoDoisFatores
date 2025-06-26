using AutenticacaoDoisFatores.Domain.Shared;
using Messenger;
using Messenger.WebApi;

namespace AutenticacaoDoisFatores.Shared
{
    public class BaseController(INotifier notifier, int? statusCodeNotifier = null) : BaseMessengerController(notifier, statusCodeNotifier)
    {
        protected static string ApiUrl(HttpContext httpContext)
        {
            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
        }

        protected static string Token(HttpRequest httpRequest)
        {
            return httpRequest.Headers.Authorization.ToString().Replace("Bearer ", "");
        }

        protected static bool IsValidSecretKey(HttpRequest httpRequest)
        {
            var expectedSecretKey = Environment.GetEnvironmentVariable("ADF_CHAVE_EXCLUSIVIDADE");
            if (expectedSecretKey!.IsNullOrEmptyOrWhiteSpaces())
                return false;

            var secretKeyInRequest = httpRequest.Headers["Chave-Exclusiva"].ToString();
            if (secretKeyInRequest!.IsNullOrEmptyOrWhiteSpaces())
                return false;

            if (!secretKeyInRequest.Equals(expectedSecretKey))
                return false;

            return true;
        }
    }
}
