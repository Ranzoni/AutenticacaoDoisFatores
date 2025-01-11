using AutenticacaoDoisFatores.Compartilhados;
using AutenticacaoDoisFatores.Servico.CasosDeUso;
using AutenticacaoDoisFatores.Servico.DTO;
using Mensageiro;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoDoisFatores.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClienteController(INotificador _notificador, int? _statusCodeNotificador = null) : ControladorBase(_notificador, _statusCodeNotificador)
    {
        [HttpPost]
        public async Task<ActionResult<ClienteCadastrado?>> CriarAsync([FromServices] CriarCliente criarCliente, NovoCliente novoCliente)
        {
            try
            {
                var url = UrlAcaoDeConfirmarCadastroDeCliente(HttpContext);

                var retorno = await criarCliente.ExecutarAsync(novoCliente, url);
                
                return CriadoComSucesso(retorno);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
