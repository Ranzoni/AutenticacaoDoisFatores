using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Service.Shared;
using AutenticacaoDoisFatores.Service.Dtos.Users;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators.TwoFactorAuths
{
    public class UserTwoFactorAuthByApp(INotifier notifier) : IUserTwoFactorAuthType
    {
        private readonly INotifier _notifier = notifier;

        public Task<TwoFactorAuthResponse?> SendAsync(User user)
        {
            if (!IsSendingValid(user))
                return Task.FromResult<TwoFactorAuthResponse?>(null);

            var token = Security.GenerateUserAuthCodeToken(user.Id);
            var response = new TwoFactorAuthResponse(token);
            return Task.FromResult<TwoFactorAuthResponse?>(response);
        }

        private bool IsSendingValid(User user)
        {
            if (!user.Active)
            {
                _notifier.AddNotFoundMessage(UserValidationMessages.UserNotFound);
                return false;
            }

            return true;
        }
    }
}
