using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Users
{
    public class RemoveUser(UserDomain dominio, INotifier notifier)
    {
        private readonly UserDomain _domain = dominio;
        private readonly INotifier _notifier = notifier;

        public async Task ExecuteAsync(Guid id)
        {
            if (!await IsDeletionValid(id))
                return;

            await _domain.RemoveAsync(id);
        }

        private async Task<bool> IsDeletionValid(Guid id)
        {
            var user = await _domain.GetByIdAsync(id);
            if (user is null || user.IsAdmin)
            {
                _notifier.AddNotFoundMessage(UserValidationMessages.UserNotFound);
                return false;
            }

            return true;
        }
    }
}
