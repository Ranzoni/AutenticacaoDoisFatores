using AutenticacaoDoisFatores.Compartilhados;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using Mensageiro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoDoisFatores.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/usuario")]
    public class UsuarioController(INotificador _notificador, int? _statusCodeNotificador = null) : ControladorBase(_notificador, _statusCodeNotificador)
    {
        [HttpPost]
        [Authorize(Policy = "CriacaoDeUsuario")]
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
        [Authorize(Policy = "AtivacaoDeUsuario")]
        public async Task<ActionResult<UsuarioCadastrado?>> AtivarAsync([FromServices] AtivarUsuario ativarUsuario, Guid idUsuario)
        {
            try
            {
                await ativarUsuario.AtivarAsync(idUsuario, true);

                return Sucesso("O usuário foi ativado com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{idUsuario}/desativar")]
        [Authorize(Policy = "DesativacaoDeUsuario")]
        public async Task<ActionResult<UsuarioCadastrado?>> DesativarAsync([FromServices] AtivarUsuario ativarUsuario, Guid idUsuario)
        {
            try
            {
                await ativarUsuario.AtivarAsync(idUsuario, false);

                return Sucesso("O usuário foi desativado com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("autenticar")]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioAutenticado?>> AutenticarAsync([FromServices] AutenticarUsuario autenticarUsuario, DadosAutenticacao dadosAutenticacao)
        {
            try
            {
                var token = await autenticarUsuario.AutenticarAsync(dadosAutenticacao);

                return Sucesso(token);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{idUsuario}/gerar-nova-senha")]
        [Authorize(Policy = "TrocarSenhaDeUsuario")]
        public async Task<ActionResult> GerarNovaSenhaAsync([FromServices] GerarNovaSenhaUsuario gerarNovaSenhaUsuario, Guid idUsuario, TrocarSenhaUsuario trocarSenhaUsuario)
        {
            try
            {
                await gerarNovaSenhaUsuario.ExecutarAsync(idUsuario, trocarSenhaUsuario.NovaSenha);

                return Sucesso("A senha foi atualizada com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
