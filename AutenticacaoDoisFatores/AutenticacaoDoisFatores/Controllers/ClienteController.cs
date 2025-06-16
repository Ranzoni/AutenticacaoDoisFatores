using AutenticacaoDoisFatores.Compartilhados;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using AutenticacaoDoisFatores.Servico.DTO.Clientes;
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
        public async Task<ActionResult<RegisteredClient?>> CriarAsync([FromServices] CreateClient criarCliente, NewClient novoCliente)
        {
            try
            {
                var urlBase = UrlDaApi(HttpContext);
                var url = $"{urlBase}/{_caminhoPaginaConfirmarCadastro}";

                var retorno = await criarCliente.ExecutarAsync(novoCliente, url);
                
                return CriadoComSucesso(retorno);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("confirmar-cadastro")]
        [Authorize(Policy = "ConfirmacaoDeCliente")]
        public async Task<ActionResult> ConfirmarCadastroAsync([FromServices] ActivateClient ativarCliente)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var idCliente = Security.GetIdFromToken(token);

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
        public async Task<ActionResult> ReenviarAsync([FromServices] ResendClientKey renviarChaveCliente, string email)
        {
            try
            {
                var urlBase = UrlDaApi(HttpContext);
                var url = $"{urlBase}/{_caminhoPaginaConfirmarCadastro}";

                await renviarChaveCliente.ResendAsync(email, url);

                return Sucesso("A nova chave de acesso foi enviada para o e-mail com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("email/{email}/enviar-confirmacao-geracao-nova-chave")]
        [AllowAnonymous]
        public async Task<ActionResult> EnviarConfirmacaoGeracaoNovaChaveAsync([FromServices] SendConfirmationOfNewClientKey renviarChaveCliente, string email)
        {
            try
            {
                var urlBase = UrlDaApi(HttpContext);
                var url = $"{urlBase}/{_caminhoPaginaConfirmarNovaChave}";

                await renviarChaveCliente.SendAsync(email, url);

                return Sucesso("O e-mail foi enviado com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("gerar-nova-chave")]
        [Authorize(Policy = "GeracaoNovaChaveCliente")]
        public async Task<ActionResult> GerarNovaChaveAcessoAsync([FromServices] GenerateClientAccessKey gerarNovaChaveAcessoCliente)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var idCliente = Security.GetIdFromToken(token);

                await gerarNovaChaveAcessoCliente.GerarNovaChaveAsync(idCliente);

                return Sucesso("A nova chave de acesso foi enviada para o e-mail do cliente.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisteredClient?>> BuscarUnicoAsync([FromServices] SearchClients buscarClientes, Guid id)
        {
            try
            {
                if (!ChaveExclusivaEstaValida(HttpContext.Request))
                    return Unauthorized();

                var resposta = await buscarClientes.GetByIdAsync(id);

                return Sucesso(resposta);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RegisteredClient>>> BuscarVariosAsync([FromServices] SearchClients buscarClientes, [FromQuery] SearchClientsFilter filtros)
        {
            try
            {
                if (!ChaveExclusivaEstaValida(HttpContext.Request))
                    return Unauthorized();

                var resposta = await buscarClientes.BuscarVariosAsync(filtros);

                return Sucesso(resposta);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
