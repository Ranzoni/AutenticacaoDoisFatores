using AutenticacaoDoisFatores.Shared;
using AutenticacaoDoisFatores.Service.UseCases.Users;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators;
using AutenticacaoDoisFatores.Service.Shared;
using AutenticacaoDoisFatores.Service.Dtos.Users;
using Messenger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoDoisFatores.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/user")]
    public class UserController(INotifier notifier, int? statusCodeNotifier = null) : BaseController(notifier, statusCodeNotifier)
    {
        [HttpPost]
        [Authorize(Policy = nameof(Security.CreateUserRole))]
        public async Task<ActionResult<RegisteredUser?>> RegisterAsync([FromServices] RegisterUser registerUser, NewUser newUser)
        {
            try
            {
                var response = await registerUser.RegisterAsync(newUser);

                return SuccessfullyCreated(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}/active")]
        [Authorize(Policy = nameof(Security.AcivateUserRole))]
        public async Task<ActionResult<RegisteredUser?>> ActivateAsync([FromServices] ActivateUser activateUser, Guid id)
        {
            try
            {
                await activateUser.ActivateAsync(id, true);

                return Success("O usuário foi ativado com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}/inactive")]
        [Authorize(Policy = nameof(Security.InactivateUserRole))]
        public async Task<ActionResult<RegisteredUser?>> InactivateAsync([FromServices] ActivateUser activateUser, Guid id)
        {
            try
            {
                await activateUser.ActivateAsync(id, false);

                return Success("O usuário foi desativado com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<ActionResult<object?>> AuthenticateAsync([FromServices] AuthenticateUser authenticateUser, AuthData authData)
        {
            try
            {
                var response = await authenticateUser.ExecuteAsync(authData);

                return Success(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("authenticate/two-factor")]
        [Authorize(Policy = nameof(Security.AuthCodeEmailSenderRole))]
        public async Task<ActionResult<AuthenticatedUser?>> AuthenticateAsync([FromServices] UserAuthenticationByCode userAuthenticationByCode, AuthCodeUser authCodeUser)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var id = Security.GetIdFromToken(token);

                var response = await userAuthenticationByCode.ExecuteAsync(id, authCodeUser.Code);
                return Success(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}/change-password")]
        [Authorize(Policy = nameof(Security.ChangeUserPasswordRole))]
        public async Task<ActionResult> GenerateNewPasswordAsync([FromServices] ChangeUserPassword changeUserPassword, Guid id, UserPasswordChange userPasswordChange)
        {
            try
            {
                await changeUserPassword.ExecuteAsync(id, userPasswordChange.NewPassword);

                return Success("A senha foi atualizada com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("change-password")]
        public async Task<ActionResult> GenerateNewPasswordAsync([FromServices] ChangeUserPassword changeUserPassword, UserPasswordChange userPasswordChange)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var id = Security.GetIdFromToken(token);

                await changeUserPassword.ExecuteAsync(id, userPasswordChange.NewPassword);

                return Success("A senha foi atualizada com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = nameof(Security.RemoveUserRole))]
        public async Task<ActionResult> RemoveAsync([FromServices] RemoveUser removeUser, Guid id)
        {
            try
            {
                await removeUser.ExecuteAsync(id);

                return Success("Usuário excluído com sucesso");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<RegisteredUser?>> UpdateAsync([FromServices] UpdateUser updateUser, NewUserData newUserData)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var id = Security.GetIdFromToken(token);

                var response = await updateUser.ExecuteAsync(id, newUserData);

                return Success(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = nameof(Security.ViewUsersRole))]
        public async Task<ActionResult<RegisteredUser?>> GetByIdAsync([FromServices] SearchUsers searchUsers, Guid id)
        {
            try
            {
                var response = await searchUsers.GetByIdAsync(id);

                return Success(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Authorize(Policy = nameof(Security.ViewUsersRole))]
        public async Task<ActionResult<IEnumerable<RegisteredUser>>> GetAllAsync([FromServices] SearchUsers searchUsers, [FromQuery] SearchUserFilters filter)
        {
            try
            {
                var response = await searchUsers.GetAllAsync(filter);

                return Success(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("data")]
        public async Task<ActionResult<RegisteredUser?>> GetDataAsync([FromServices] SearchUsers searchUsers)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var id = Security.GetIdFromToken(token);

                var response = await searchUsers.GetByIdAsync(id);

                return Success(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("change-email")]
        public async Task<ActionResult> ChangeEmailAsync([FromServices] ChangeUserEmail changeUserEmail, UserEmailChange userEmailChange)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var id = Security.GetIdFromToken(token);

                await changeUserEmail.ExecuteAsync(id, userEmailChange);

                return Success("O e-mail foi alterado com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("generate-qr-code")]
        public async Task<ActionResult> GenerateTwoFactorAuthQrCodeAsync([FromServices] GenerateAuthAppQrCode generateAuthAppQrCode)
        {
            try
            {
                var token = Token(HttpContext.Request);
                var id = Security.GetIdFromToken(token);

                await generateAuthAppQrCode.ExecuteAsync(id);

                return Success("O QrCode foi gerado com sucesso e enviado ao e-mail do usuário.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
