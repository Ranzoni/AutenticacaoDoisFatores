using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Shared.Users;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators;
using AutenticacaoDoisFatores.Service.Dtos.Users;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Users
{
    public class UserAuthenticationByCode(AuthCodeManager authCodeManager, UserDomain userDomain, BaseUserAuthenticator baseUserAuthenticator, AuthApp authApp, INotifier notifier)
    {
        private readonly AuthCodeManager _authCodeManager = authCodeManager;
        private readonly UserDomain _userDomain = userDomain;
        private readonly BaseUserAuthenticator _baseUserAuthenticator = baseUserAuthenticator;
        private readonly AuthApp _authApp = authApp;
        private readonly INotifier _notifier = notifier;

        public async Task<AuthenticatedUser?> ExecuteAsync(Guid userId, string code)
        {
            var user = await _userDomain.GetByIdAsync(userId);
            if (!IsUserValid(user))
                return null;

            if (!await IsCodeValidAsync(user!, code))
                return null;

            var authenticatedUser = await _baseUserAuthenticator.ExecuteAsync(user!);
            return (AuthenticatedUser?)authenticatedUser;
        }

        private bool IsUserValid(User? user)
        {
            if (user is null || !user.Active)
            {
                _notifier.AddUnauthorizedMessage(UserValidationMessages.UserNotFound);
                return false;
            }

            return true;
        }

        private async Task<bool> IsCodeValidAsync(User user, string code)
        {
            if (user.AuthType.Equals(AuthType.AuthApp))
                return ValidateCodeByApp(code, user);
            else
            {
                var isValid = await ValidateCodeWithSavedAsync(user.Id, code);
                if (isValid)
                    await _authCodeManager.RemoveCodeAsync(user.Id);

                return isValid;
            }
        }

        private bool ValidateCodeByApp(string code, User user)
        {
            if (!_authApp.IsCodeValid(code, user))
            {
                _notifier.AddUnauthorizedMessage(UserValidationMessages.Unauthorized);
                return false;
            }

            return true;
        }

        private async Task<bool> ValidateCodeWithSavedAsync(Guid userId, string code)
        {
            var codeSaved = await _authCodeManager.GetCodeAsync(userId);
            var encryptedCode = Encrypt.EncryptWithSha512(code);
            if (!encryptedCode.Equals(codeSaved))
            {
                _notifier.AddUnauthorizedMessage(UserValidationMessages.Unauthorized);
                return false;
            }

            return true;
        }
    }
}
