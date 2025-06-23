using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Shared.Permissions;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Service.Shared;
using AutenticacaoDoisFatores.Service.Dtos.Users;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators
{
    public class BaseUserAuthenticator(UserDomain dominio, PermissionsDomain permissionsDomain, INotifier notifier) : IAuthType
    {
        private readonly UserDomain _dominio = dominio;
        private readonly PermissionsDomain _permissionsDomain = permissionsDomain;
        private readonly INotifier _notifier = notifier;

        public async Task<object?> ExecuteAsync(User user)
        {
            if (!IsAuthenticationValid(user))
                return null;

            var userPermissions = await GetUserPermissionsAsync(user);
            var token = Security.GenerateUserAuthToken(user, userPermissions);

            user.UpdateLastAccess();
            await _dominio.UpdateAsync(user);

            var registeredUser = (RegisteredUser)user;
            return new AuthenticatedUser(user: registeredUser, token: token);
        }

        private bool IsAuthenticationValid(User user)
        {
            if (!user.Active)
            {
                _notifier.AddNotFoundMessage(UserValidationMessages.UserNotFound);
                return false;
            }

            return true;
        }

        private async Task<IEnumerable<PermissionType>> GetUserPermissionsAsync(User user)
        {
            if (user.IsAdmin)
                return PermissionsDomain.GetAll();
                
            return await _permissionsDomain.GetByUserIdAsync(user.Id);
        }
    }
}
