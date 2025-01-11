using Mensageiro;
using Mensageiro.WebApi;

namespace AutenticacaoDoisFatores.Compartilhados
{
    public class ControladorBase(INotificador _notificador, int? _statusCodeNotificador = null) : MensageiroControllerBase(_notificador, _statusCodeNotificador)
    {
        protected static string UrlAcaoDeConfirmarCadastroDeCliente(HttpContext httpContext)
        {
            return $"{UrlDaApi(httpContext)}/Cliente/ConfirmarCadastroCliente";
        }

        private static string UrlDaApi(HttpContext httpContext)
        {
            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
        }
    }
}
