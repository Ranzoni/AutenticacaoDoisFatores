using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators
{
    public class GenerateAuthAppQrCode(AuthApp authApp, UserDomain userDomain, EmailSender email, INotifier notifier, string baseLinkToQrCode)
    {
        private readonly UserDomain _userDomain = userDomain;
        private readonly AuthApp _authApp = authApp;
        private readonly EmailSender _email = email;
        private readonly INotifier _notifier = notifier;

        public async Task ExecuteAsync(Guid userId)
        {
            var user = await _userDomain.GetByIdAsync(userId);
            if (!IsGenerationValid(user))
                return;

            var qrCode = _authApp.GenerateQrCode(user!);
            if (!IsQrCodeValid(qrCode))
                return;

            _email.SendTwoFactorAuthQrCode(user!.Email, baseLinkToQrCode, qrCode!);
        }

        private bool IsGenerationValid(User? user)
        {
            if (user is null || !user.Active)
            {
                _notifier.AddNotFoundMessage(UserValidationMessages.UserNotFound);
                return false;
            }

            return true;
        }

        private bool IsQrCodeValid(string? qrCode)
        {
            if (qrCode is null || qrCode.IsNullOrEmptyOrWhiteSpaces())
            {
                _notifier.AddMessage(AuthAppValidationMessages.QrCodeNotGenerated);
                return false;
            }

            return true;
        }
    }
}
