using AutenticacaoDoisFatores.Shared;
using AutenticacaoDoisFatores.Service.UseCases.Clients;
using AutenticacaoDoisFatores.Service.Shared;
using AutenticacaoDoisFatores.Service.Dtos.Clients;
using Messenger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoDoisFatores.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/client")]
    public class ClientController(INotifier notifier, int? statusCodeNotifier = null) : BaseController(notifier, statusCodeNotifier)
    {
        private readonly string _registerConfirmationPagePath = "clients/register-confirmation.html";
        private readonly string _newKeyConfirmationPagePath = "clients/new-access-key-confirmation.html";

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<RegisteredClient?>> CreateAsync([FromServices] CreateClient createClient, NewClient newClient)
        {
            try
            {
                var baseUrl = ApiUrl(HttpContext);
                var url = $"{baseUrl}/{_registerConfirmationPagePath}";

                var response = await createClient.ExecuteAsync(newClient, url);
                
                return SuccessfullyCreated(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("confirm-register")]
        [Authorize(Policy = nameof(Security.ClientConfirmationRole))]
        public async Task<ActionResult> ConfirmRegisterAsync([FromServices] ActivateClient activateClient)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var clientId = Security.GetIdFromToken(token);

                await activateClient.ActivateAsync(clientId);

                return Success("Confirmação de cadastro realizada com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("email/{email}/resend-key")]
        [AllowAnonymous]
        public async Task<ActionResult> ResendAsync([FromServices] ResendClientKey resendClientKey, string email)
        {
            try
            {
                var baseUrl = ApiUrl(HttpContext);
                var url = $"{baseUrl}/{_registerConfirmationPagePath}";

                await resendClientKey.ResendAsync(email, url);

                return Success("A nova chave de acesso foi enviada para o e-mail com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("email/{email}/send-new-key-generation-confirmation")]
        [AllowAnonymous]
        public async Task<ActionResult> SendConfirmationOfNewClientKeyAsync([FromServices] SendConfirmationOfNewClientKey sendConfirmationOfNewClientKey, string email)
        {
            try
            {
                var baseUrl = ApiUrl(HttpContext);
                var url = $"{baseUrl}/{_newKeyConfirmationPagePath}";

                await sendConfirmationOfNewClientKey.SendAsync(email, url);

                return Success("O e-mail foi enviado com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("generate-new-key")]
        [Authorize(Policy = nameof(Security.NewClientKeyGenerationRole))]
        public async Task<ActionResult> GenerateNewAccessKeyAsync([FromServices] GenerateClientAccessKey generateClientAccessKey)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var clientId = Security.GetIdFromToken(token);

                await generateClientAccessKey.GenerateAsync(clientId);

                return Success("A nova chave de acesso foi enviada para o e-mail do cliente.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisteredClient?>> GetByIdAsync([FromServices] SearchClients searchClients, Guid id)
        {
            try
            {
                if (!IsValidSecretKey(HttpContext.Request))
                    return Unauthorized();

                var response = await searchClients.GetByIdAsync(id);

                return Success(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RegisteredClient>>> GetAllAsync([FromServices] SearchClients searchClients, [FromQuery] SearchClientsFilter filter)
        {
            try
            {
                if (!IsValidSecretKey(HttpContext.Request))
                    return Unauthorized();

                var response = await searchClients.GetAllAsync(filter);

                return Success(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
