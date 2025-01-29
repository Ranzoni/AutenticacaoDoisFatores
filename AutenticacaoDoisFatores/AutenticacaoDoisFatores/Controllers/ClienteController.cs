using AutenticacaoDoisFatores.Compartilhados;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using AutenticacaoDoisFatores.Servico.DTO;
using Mensageiro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoDoisFatores.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/cliente")]
    public class ClienteController(INotificador _notificador, int? _statusCodeNotificador = null) : ControladorBase(_notificador, _statusCodeNotificador)
    {
        private readonly string _caminhoPaginaConfirmarCadastro = "clientes/confirmar-cadastro.html";
        private readonly string _caminhoPaginaConfirmarNovaChave = "clientes/confirmar-geracao-nova-chave.html";

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ClienteCadastrado?>> CriarAsync([FromServices] CriarCliente criarCliente, NovoCliente novoCliente)
        {
            try
            {
                var urlBase = UrlDaApi(HttpContext);
                var url = $"{urlBase}/{_caminhoPaginaConfirmarCadastro}";

                var retorno = await criarCliente.CriarAsync(novoCliente, url);
                
                return CriadoComSucesso(retorno);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("confirmar-cadastro")]
        [Authorize(Policy = "ConfirmacaoDeCliente")]
        public async Task<ActionResult> ConfirmarCadastroAsync([FromServices] AtivarCliente ativarCliente)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var idCliente = Seguranca.RetornarIdClienteDoToken(token);

                await ativarCliente.AtivarAsync(idCliente);

                return Sucesso("Confirmação de cadastro realizada com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("email/{email}/reenviar-chave")]
        [AllowAnonymous]
        public async Task<ActionResult> ReenviarAsync([FromServices] ReenviarChaveCliente renviarChaveCliente, string email)
        {
            try
            {
                var urlBase = UrlDaApi(HttpContext);
                var url = $"{urlBase}/{_caminhoPaginaConfirmarCadastro}";

                await renviarChaveCliente.ReenviarAsync(email, url);

                return Sucesso("A nova chave de acesso foi enviada para o e-mail com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("email/{email}/enviar-confirmacao-geracao-nova-chave")]
        [AllowAnonymous]
        public async Task<ActionResult> EnviarConfirmacaoGeracaoNovaChaveAsync([FromServices] EnviarConfirmacaoNovaChaveCliente renviarChaveCliente, string email)
        {
            try
            {
                var urlBase = UrlDaApi(HttpContext);
                var url = $"{urlBase}/{_caminhoPaginaConfirmarNovaChave}";

                await renviarChaveCliente.EnviarAsync(email, url);

                return Sucesso("O e-mail foi enviado com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("gerar-nova-chave")]
        [Authorize(Policy = "GeracaoNovaChaveCliente")]
        public async Task<ActionResult> GerarNovaChaveAcessoAsync([FromServices] GerarNovaChaveAcessoCliente gerarNovaChaveAcessoCliente)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var idCliente = Seguranca.RetornarIdClienteDoToken(token);

                await gerarNovaChaveAcessoCliente.GerarNovaChaveAsync(idCliente);

                return Sucesso("A nova chave de acesso foi enviada para o e-mail do cliente.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
