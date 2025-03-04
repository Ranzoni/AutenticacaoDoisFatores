using AutenticacaoDoisFatores.Compartilhados;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Permissoes;
using AutenticacaoDoisFatores.Servico.DTO.Permissoes;
using Mensageiro;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoDoisFatores.Controllers
{
    [ApiController]
    [Route("api/permissao")]
    public class PermissaoController(INotificador _notificador, int? _statusCodeNotificador = null) : ControladorBase(_notificador, _statusCodeNotificador)
    {
        [HttpGet]
        public ActionResult<IEnumerable<PermissaoDisponivel>> RetornarPermissoes()
        {
            try
            {
                var permissoes = RetornarTodasPermissoes.Executar();

                return Sucesso(permissoes);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
