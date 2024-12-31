using AutenticacaoDoisFatores.Servico.CasosDeUso;
using AutenticacaoDoisFatores.Servico.DTO;
using Mensageiro;
using Mensageiro.WebApi;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoDoisFatores.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClienteController(INotificador _notificador, int? _statusCodeNotificador = null) : MensageiroControllerBase(_notificador, _statusCodeNotificador)
    {
        [HttpPost]
        public async Task<ActionResult<ClienteCadastrado?>> CriarAsync([FromServices] CriarCliente criarCliente, NovoCliente novoCliente)
        {
            try
            {
                var retorno = await criarCliente.ExecutarAsync(novoCliente);

                return CriadoComSucesso(retorno);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
