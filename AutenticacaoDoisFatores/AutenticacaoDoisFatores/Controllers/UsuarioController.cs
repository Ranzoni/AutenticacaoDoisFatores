using AutenticacaoDoisFatores.Compartilhados;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios;
using AutenticacaoDoisFatores.Servico.Compartilhados;
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
        public async Task<ActionResult<object?>> AutenticarAsync([FromServices] AutenticarUsuario autenticarUsuario, DadosAutenticacao dadosAutenticacao)
        {
            try
            {
                var resposta = await autenticarUsuario.ExecutarAsync(dadosAutenticacao);

                return Sucesso(resposta);
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

        [HttpDelete("{idUsuario}")]
        [Authorize(Policy = "ExclusaoDeUsuario")]
        public async Task<ActionResult> ExcluirAsync([FromServices] ExcluirUsuario excluirUsuario, Guid idUsuario)
        {
            try
            {
                await excluirUsuario.ExecutarAsync(idUsuario);

                return Sucesso("Usuário excluído com sucesso");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<UsuarioCadastrado?>> AlterarAsync([FromServices] AlterarUsuario alterarUsuario, NovosDadosUsuario novosDadosUsuario)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var idUsuario = Seguranca.RetornarIdDoToken(token);

                var retorno = await alterarUsuario.ExecutarAsync(idUsuario, novosDadosUsuario);

                return Sucesso(retorno);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "VisualizacaoDeUsuarios")]
        public async Task<ActionResult<UsuarioCadastrado?>> BuscarUnicoAsync([FromServices] BuscarUsuarios buscarUsuarios, Guid id)
        {
            try
            {
                var resposta = await buscarUsuarios.BuscarUnicoAsync(id);

                return Sucesso(resposta);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Authorize(Policy = "VisualizacaoDeUsuarios")]
        public async Task<ActionResult<IEnumerable<UsuarioCadastrado>>> BuscarVariosAsync([FromServices] BuscarUsuarios buscarUsuarios, [FromQuery] FiltrosParaBuscarUsuario filtros)
        {
            try
            {
                var resposta = await buscarUsuarios.BuscarVariosAsync(filtros);

                return Sucesso(resposta);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("dados")]
        public async Task<ActionResult<UsuarioCadastrado?>> RetornarDadosAsync([FromServices] BuscarUsuarios buscarUsuarios)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var id = Seguranca.RetornarIdDoToken(token);

                var resposta = await buscarUsuarios.BuscarUnicoAsync(id);

                return Sucesso(resposta);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{idUsuario}/trocar-email")]
        [Authorize(Policy = "TrocarEmailDeUsuario")]
        public async Task<ActionResult> AlterarEmailAsync([FromServices] AlterarUsuario alterarUsuario, Guid idUsuario, TrocarEmailUsuario trocarEmailUsuario)
        {
            try
            {
                var novosDadosUsuario = (NovosDadosUsuario)trocarEmailUsuario;

                await alterarUsuario.ExecutarAsync(idUsuario, novosDadosUsuario);

                return Sucesso("O e-mail foi alterado com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
