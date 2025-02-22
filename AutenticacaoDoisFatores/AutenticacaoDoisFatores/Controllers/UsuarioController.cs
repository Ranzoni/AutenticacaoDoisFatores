using AutenticacaoDoisFatores.Compartilhados;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using Mensageiro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoDoisFatores.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/usuario")]
    public class UsuarioController(INotificador _notificador, int? _statusCodeNotificador = null) : ControladorBase(_notificador, _statusCodeNotificador)
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioCadastrado?>> CriarAsync([FromServices] CriarUsuario criarUsuario, NovoUsuario novoUsuario)
        {
            try
            {
                var retorno = await criarUsuario.CriarAsync(novoUsuario);

                return CriadoComSucesso(retorno);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
