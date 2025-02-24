using AutenticacaoDoisFatores.Compartilhados;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using Mensageiro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoDoisFatores.Controllers
{
    [ApiController]
    [Route("api/usuario")]
    public class UsuarioController(INotificador _notificador, int? _statusCodeNotificador = null) : ControladorBase(_notificador, _statusCodeNotificador)
    {
        [HttpPost]
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

        [HttpPut("{idUsuario}/ativar")]
        public async Task<ActionResult<UsuarioCadastrado?>> AtivarAsync([FromServices] AtivarUsuario ativarUsuario, Guid idUsuario)
        {
            try
            {
                await ativarUsuario.AtivarAsync(idUsuario);

                return Sucesso("O usuário foi ativado com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
