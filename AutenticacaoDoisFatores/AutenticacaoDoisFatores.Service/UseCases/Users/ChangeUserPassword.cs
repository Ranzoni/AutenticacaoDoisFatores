using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Service.Shared;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Users
{
    public class ChangeUserPassword(UserDomain domain, INotifier notifier)
    {
        private readonly UserDomain _domain = domain;
        private readonly INotifier _notifier = notifier;

        public async Task ExecuteAsync(Guid userId, string newPassword)
        {
            if (!IsValidPassword(newPassword))
                return;

            var user = await _domain.GetByIdAsync(userId);
            if (!IsChangeValid(user) || user is null)
                return;

            var newPassEncrypted = Encrypt.EncryptWithSha512(newPassword);
            user.UpdatePassword(newPassEncrypted);
            await _domain.UpdateAsync(user);
        }

        private bool IsValidPassword(string newPassword)
        {
            if (!Security.IsPasswordValid(newPassword))
            {
                _notifier.AddMessage(UserValidationMessages.InvalidPassword);
                return false;
            }

            return true;
        }

        private bool IsChangeValid(User? user)
        {
            if (user is null || !user.Active || user.IsAdmin)
            {
                _notifier.AddNotFoundMessage(UserValidationMessages.UserNotFound);
                return false;
            }

            return !_notifier.AnyMessage();
        }
    }
}
