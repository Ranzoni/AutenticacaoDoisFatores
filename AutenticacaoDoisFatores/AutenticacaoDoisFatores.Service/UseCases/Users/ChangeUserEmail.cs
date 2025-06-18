using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Validators;
using AutenticacaoDoisFatores.Service.Dtos.Users;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Users
{
    public class ChangeUserEmail(UserDomain domain, INotifier notifier)
    {
        private readonly UserDomain _domain = domain;
        private readonly INotifier _notifier = notifier;

        public async Task ExecuteAsync(Guid userId, UserEmailChange userEmailChange)
        {
            var user = await _domain.GetByIdAsync(userId);
            if (!await IsChangeValidAsync(user, userEmailChange))
                return;

            user!.UpdateEmail(userEmailChange.NewEmail);
            await _domain.UpdateAsync(user);
        }

        private async Task<bool> IsChangeValidAsync(User? user, UserEmailChange userEmailChange)
        {
            if (user is null || !user.Active || user.IsAdmin)
                _notifier.AddNotFoundMessage(UserValidationMessages.UserNotFound);

            if (!UserValidator.IsEmailValid(userEmailChange.NewEmail))
                _notifier.AddMessage(UserValidationMessages.InvalidEmail);
            else
            {
                var emailExists = await _domain.EmailExistsAsync(userEmailChange.NewEmail);
                if (emailExists)
                    _notifier.AddMessage(UserValidationMessages.EmailAlreadyRegistered);
            }

            return !_notifier.AnyMessage();
        }
    }
}
