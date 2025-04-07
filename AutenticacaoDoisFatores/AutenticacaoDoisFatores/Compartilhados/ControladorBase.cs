using AutenticacaoDoisFatores.Dominio.Compartilhados;
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

        protected static bool ChaveExclusivaEstaValida(HttpRequest httpRequest)
        {
            var chaveExclusividade = Environment.GetEnvironmentVariable("ADF_CHAVE_EXCLUSIVIDADE");
            if (chaveExclusividade!.EstaVazio())
                return false;

            var chaveRequisicao = httpRequest.Headers["Chave-Exclusiva"].ToString();
            if (chaveRequisicao!.EstaVazio())
                return false;

            if (!chaveRequisicao.Equals(chaveExclusividade))
                return false;

            return true;
        }
    }
}
