using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Shared.Permissions;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Permissions
{
    public class AddUserPermission(PermissionsDomain domain, UserDomain userDomain, INotifier notifier)
    {
        private readonly PermissionsDomain _domain = domain;
        private readonly UserDomain _userDomain = userDomain;
        private readonly INotifier _notifier = notifier;

        public async Task ExecuteAsync(Guid userId, IEnumerable<PermissionType> permissionsToInclude)
        {
            var user = await _userDomain.GetByIdAsync(userId);
            if (!IsPermissionInclusionValid(user))
                return;

            await _domain.AddAsync(userId, permissionsToInclude);
        }

        private bool IsPermissionInclusionValid(User? user)
        {
            if (user is null || !user.Active || user.IsAdmin)
            {
                _notifier.AddNotFoundMessage(UserValidationMessages.UserNotFound);
                return false;
            }

            return true;
        }
    }
}
