using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Service.Dtos.Users;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators.TwoFactorAuths;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators
{
    public class ResendAuthCode(UserDomain userDomain, UserTwoFactorAuthByEmail userTwoFactorAuthByEmail, INotifier notifier)
    {
        private readonly UserDomain _userDomain = userDomain;
        private readonly UserTwoFactorAuthByEmail _userTwoFactorAuthByEmail = userTwoFactorAuthByEmail;
        private readonly INotifier _notifier = notifier;

        public async Task<TwoFactorAuthResponse?> ResendAsync(Guid userId)
        {
            var user = await _userDomain.GetByIdAsync(userId);
            if (!IsResendValid(user))
                return null;

            return await _userTwoFactorAuthByEmail.SendAsync(user!);
        }

        private bool IsResendValid(User? user)
        {
            if (user is null || !user.Active)
            {
                _notifier.AddNotFoundMessage(UserValidationMessages.UserNotFound);
                return false;
            }

            return true;
        }
    }
}
