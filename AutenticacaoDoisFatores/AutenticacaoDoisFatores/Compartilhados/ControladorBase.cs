using Mensageiro;
using Mensageiro.WebApi;

namespace AutenticacaoDoisFatores.Compartilhados
{
    public class ControladorBase(INotificador _notificador, int? _statusCodeNotificador = null) : MensageiroControllerBase(_notificador, _statusCodeNotificador)
    {
        protected static string UrlDaApi(HttpContext httpContext)
        {
            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
        }

        protected static string Token(HttpRequest httpRequest)
        {
            return httpRequest.Headers.Authorization.ToString().Replace("Bearer ", "");
        }
    }
}
