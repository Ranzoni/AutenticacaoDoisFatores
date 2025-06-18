using AutenticacaoDoisFatores.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Permissions;
using AutenticacaoDoisFatores.Service.UseCases.Permissions;
using AutenticacaoDoisFatores.Service.Dtos.Permissions;
using Messenger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutenticacaoDoisFatores.Service.Shared;

namespace AutenticacaoDoisFatores.Controllers
{
    [ApiController]
    [Authorize(Policy = nameof(Security.SetPermissionsRole))]
    [Route("api/permission")]
    public class PermissionController(INotifier notifier, int? statusCodeNotifier = null) : BaseController(notifier, statusCodeNotifier)
    {
        [HttpGet]
        public ActionResult<IEnumerable<AvaiblePermission>> GetAll()
        {
            try
            {
                var permissions = GetPermissions.GetAll();

                return Success(permissions);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("user/{userId}")]
        public async Task<ActionResult> IncluirParaUsuarioAsync([FromServices] AddUserPermission addUserPermission, Guid userId, IEnumerable<PermissionType> permissions)
        {
            try
            {
                await addUserPermission.ExecuteAsync(userId, permissions);

                return Success("As permissões foram incluídas com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("usuario/{userId}")]
        public async Task<ActionResult<IEnumerable<AvaiblePermission>>> GetByUserIdAsync([FromServices] GetPermissions getPermissions, Guid userId)
        {
            try
            {
                var response = await getPermissions.GetByUserIdAsync(userId);

                return Success(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("user/{userId}")]
        public async Task<ActionResult> RemovePermissionUserAsync([FromServices] RemoveUserPermission removeUserPermission, Guid userId, IEnumerable<PermissionType> permissions)
        {
            try
            {
                await removeUserPermission.ExecuteAsync(userId, permissions);

                return Success("As permissões foram removidas com sucesso!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
