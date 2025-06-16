using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Service.Dtos.Users;
using Messenger;
using AutenticacaoDoisFatores.Service.Shared;

namespace AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators.TwoFactorAuths
{
    public class UserTwoFactorAuthByEmail(EmailSender email, AuthCodeManager authCodeManager, INotifier notifier) : IUserTwoFactorAuthType
    {
        private readonly EmailSender _email = email;
        private readonly INotifier _notifier = notifier;
        private readonly AuthCodeManager _authCodeManager = authCodeManager;

        public async Task<TwoFactorAuthResponse?> SendAsync(User user)
        {
            if (!IsEmailValid(user))
                return null;

            var authCode = Security.GenerateAuthCode();
            var encryptedAuthCode = Encrypt.EncryptWithSha512(authCode);
            
            await _authCodeManager.SaveAsync(user.Id, encryptedAuthCode);
            _email.SendTwoFactorAuthCode(user.Email, authCode);

            var token = Security.GenerateUserAuthCodeToken(user.Id);
            var response = new TwoFactorAuthResponse(token);
            return response;
        }

        private bool IsEmailValid(User user)
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
