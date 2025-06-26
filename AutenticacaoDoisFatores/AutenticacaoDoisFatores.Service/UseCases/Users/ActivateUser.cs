using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Users
{
    public class ActivateUser(UserDomain domain, INotifier notifier)
    {
        private readonly UserDomain _domain = domain;
        private readonly INotifier _notifier = notifier;

        public async Task ActivateAsync(Guid userId, bool activate)
        {
            var user = await _domain.GetByIdAsync(userId);
            if (!IsActivationValid(user) || user is null)
                return;

            user.SetActive(activate);
            await _domain.UpdateAsync(user);
        }

        public bool IsActivationValid(User? user)
        {
            if (user is null || user.IsAdmin)
            {
                _notifier.AddNotFoundMessage(UserValidationMessages.UserNotFound);
                return false;
            }

            return !_notifier.AnyMessage();
        }
    }
}
