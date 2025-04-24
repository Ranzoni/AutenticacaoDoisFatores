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
        public ActionResult<IEnumerable<PermissaoDisponivel>> RetornarTodas()
        {
            try
            {
                var permissoes = Servico.CasosDeUso.Permissoes.RetornarPermissoes.RetornarTodas();

                return Sucesso(permissoes);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("usuario/{idUsuario}")]
        public async Task<ActionResult> IncluirParaUsuarioAsync([FromServices] IncluirPermissoesParaUsuario incluirPermissoesParaUsuario, Guid idUsuario, IEnumerable<TipoDePermissao> permissoes)
        {
            try
            {
                await incluirPermissoesParaUsuario.ExecutarAsync(idUsuario, permissoes);

                return Sucesso("As permissões foram incluídas com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<PermissaoDisponivel>>> RetornarPorUsuarioAsync([FromServices] RetornarPermissoes retornarPermissoes, Guid idUsuario)
        {
            try
            {
                var retorno = await retornarPermissoes.RetornarPorUsuarioAsync(idUsuario);

                return Sucesso(retorno);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("usuario/{idUsuario}")]
        public async Task<ActionResult> RemoverParaUsuarioAsync([FromServices] RemoverPermissoesParaUsuario removerPermissoesParaUsuario, Guid idUsuario, IEnumerable<TipoDePermissao> permissoes)
        {
            try
            {
                await removerPermissoesParaUsuario.ExecutarAsync(idUsuario, permissoes);

                return Sucesso("As permissões foram removidas com sucesso!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
