using AutenticacaoDoisFatores.Compartilhados;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores;
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

        [HttpPost("autenticar/dois-fatores")]
        [Authorize(Policy = "CodAutenticaoPorEmail")]
        public async Task<ActionResult<UsuarioAutenticado?>> AutenticarAsync([FromServices] AutenticarUsuarioPorCodigo autenticarUsuario, CodigoAuntenticacaoUsuario codigoAuntenticacaoUsuario)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var idUsuario = Seguranca.RetornarIdDoToken(token);

                var retorno = await autenticarUsuario.ExecutarAsync(idUsuario, codigoAuntenticacaoUsuario.Codigo);
                return Sucesso(retorno);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{idUsuario}/alterar-senha")]
        [Authorize(Policy = "TrocarSenhaDeUsuario")]
        public async Task<ActionResult> GerarNovaSenhaAsync([FromServices] AlterarSenhaUsuario gerarNovaSenhaUsuario, Guid idUsuario, TrocarSenhaUsuario trocarSenhaUsuario)
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

        [HttpPut("alterar-senha")]
        public async Task<ActionResult> GerarNovaSenhaAsync([FromServices] AlterarSenhaUsuario gerarNovaSenhaUsuario, TrocarSenhaUsuario trocarSenhaUsuario)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var idUsuario = Seguranca.RetornarIdDoToken(token);

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
        public async Task<ActionResult> AlterarEmailAsync([FromServices] AlterarEmailUsuario alterarEmailUsuario, Guid idUsuario, TrocarEmailUsuario trocarEmailUsuario)
        {
            try
            {
                await alterarEmailUsuario.ExecutarAsync(idUsuario, trocarEmailUsuario);

                return Sucesso("O e-mail foi alterado com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("trocar-email")]
        public async Task<ActionResult> AlterarEmailAsync([FromServices] AlterarEmailUsuario alterarEmailUsuario, TrocarEmailUsuario trocarEmailUsuario)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var idUsuario = Seguranca.RetornarIdDoToken(token);

                await alterarEmailUsuario.ExecutarAsync(idUsuario, trocarEmailUsuario);

                return Sucesso("O e-mail foi alterado com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("gerar-qr-code")]
        public async Task<ActionResult> GerarQrCodeAppAutenticacaoAsync([FromServices] GerarQrCodeAppAutenticacao gerarQrCodeAppAutenticacao)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var id = Seguranca.RetornarIdDoToken(token);

                await gerarQrCodeAppAutenticacao.ExecutarAsync(id);

                return Sucesso("O QrCode foi gerado com sucesso e enviado ao e-mail do usuário.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{idUsuario}/enviar-email")]
        public async Task<ActionResult> EnviarEmailParaUsuarioAsync([FromServices] EnviarEmailParaUsuario enviarEmailAtivacao, Guid idUsuario, EnvioEmailParaUsuario envioEmailAtivacaoUsuario)
        {
            try
            {
                await enviarEmailAtivacao.ExecutarAsync(idUsuario, envioEmailAtivacaoUsuario);

                return Sucesso("O e-mail foi enviado com sucesso!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
