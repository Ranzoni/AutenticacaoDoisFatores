using AutenticacaoDoisFatores.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Permissoes;
using AutenticacaoDoisFatores.Servico.DTO.Permissoes;
using Mensageiro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoDoisFatores.Controllers
{
    [ApiController]
    [Authorize(Policy = "DefinirPermissoes")]
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

        [HttpPost("usuario/{idUsuario}")]
        public async Task<ActionResult> IncluirPermissoesUsuarioAsync([FromServices] IncluirPermissoesParaUsuario incluirPermissoesParaUsuario, Guid idUsuario, IEnumerable<TipoDePermissao> permissoes)
        {
            try
            {
                await incluirPermissoesParaUsuario.ExecuteAsync(idUsuario, permissoes);

                return Sucesso("Permissões incluídas com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
