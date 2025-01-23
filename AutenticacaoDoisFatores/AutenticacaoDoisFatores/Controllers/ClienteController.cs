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
                var idCliente = Seguranca.RetornarIdClienteTokenDeConfirmacaoDeCliente(token);

                await ativarCliente.AtivarAsync(idCliente);

                return Sucesso("Confirmação de cadastro realizada com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
