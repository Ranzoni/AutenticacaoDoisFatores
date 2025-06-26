using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Exceptions;
using AutenticacaoDoisFatores.Domain.Services;

namespace AutenticacaoDoisFatores.Domain.Domains
{
    public class EmailSender(IEmailService service)
    {
        private readonly IEmailService _service = service;

        public void SendTwoFactorAuthQrCode(string to, string linkToQrCodeGeneration, string qrCode)
        {
            var subject = EmailMessages.TwoFactorAuthCodeSubject.Description() ?? "";
            var message = EmailMessages.TwoFactorAuthCodeMessage.Description() ?? "";
            var linkMessage = EmailMessages.GenerateQrCodeMessage.Description() ?? "";

            var emailMessage = GenerateEmailMessage
                (
                    message: message,
                    details: @$"
                        <a href='{linkToQrCodeGeneration}?qrCode={qrCode}'>
                            {linkMessage}
                        </a>
                    "
                );

            _service.Send(to: to, subject: subject, message: emailMessage);
        }

        public void SendTwoFactorAuthCode(string to, string code)
        {
            var subject = EmailMessages.TwoFactorAuthTokenSubject.Description() ?? "";
            var message = EmailMessages.TwoFactorAuthTokenMessage.Description() ?? "";

            var emailMessage = GenerateEmailMessage
                (
                    message: message,
                    details: @$"

                        <p style='font-size: 18px; color: #666666;'>
                            {code}
                        </p>
                    "
                );

            _service.Send(to: to, subject: subject, message: emailMessage);
        }

        public void SendClientConfirmation(string to, string accessKey, string confirmationLink, string token)
        {
            if (to.IsNullOrEmptyOrWhiteSpaces() || !to.IsValidEmail())
                EmailException.InvalidDestinationEmail();

            var subject = EmailMessages.ClientConfirmationSubject.Description() ?? "";
            var message = EmailMessages.ClientConfirmationMessage.Description() ?? "";
            var accessKeyMessage = EmailMessages.ThisIsYoursAccessKey.Description() ?? "";
            var confirmationAccessKeyMessage = EmailMessages.ToConfirmAccessKeyMessage.Description() ?? "";
            var linkMessage = EmailMessages.ClientConfirmationLinkMessage.Description() ?? "";

            var emailMessage = GenerateEmailMessage
                (
                    message: message,
                    details: @$"
                        <form id='form-confirmar-cadastro'>

                            <p style='font-size: 18px; color: #666666;'>
                                {confirmationAccessKeyMessage}
                            </p>

                            <a href='{confirmationLink}?token={token}'>
                                {linkMessage}
                            </a>

                        </form>

                        <p style='font-size: 18px; color: #666666;'>{accessKeyMessage}</p>

                        <p style='font-size: 18px; color: #666666;'>{accessKey}</p>"
                );

            _service.Send(to: to, subject: subject, message: emailMessage);
        }

        public void SendNewAcessKeyGeneration(string to, string confirmationLink, string token)
        {
            if (to.IsNullOrEmptyOrWhiteSpaces() || !to.IsValidEmail())
                EmailException.InvalidDestinationEmail();

            var subject = EmailMessages.NewAccessKeySubject.Description() ?? "";
            var generationAccessKeyMessage = EmailMessages.ToNewAccessKeyMessage.Description() ?? "";
            var linkMessage = EmailMessages.NewAccessKeyLinkMessage.Description() ?? "";

            var emailMessage = GenerateEmailMessage
                (
                    message: generationAccessKeyMessage,
                    details: @$"
                        <form id='form-confirmar-cadastro'>

                            <a href='{confirmationLink}?token={token}'>
                                {linkMessage}
                            </a>

                        </form>"
                );

            _service.Send(to: to, subject: subject, message: emailMessage);
        }

        public void SendNewAccessKeyConfirmation(string to, string acessKey)
        {
            if (to.IsNullOrEmptyOrWhiteSpaces() || !to.IsValidEmail())
                EmailException.InvalidDestinationEmail();

            var subject = EmailMessages.NewAccessKeyConfirmationSubject.Description() ?? "";
            var message = EmailMessages.NewAccessKeyConfirmationMessage.Description() ?? "";
            var yoursAccessKeyMessage = EmailMessages.ThisIsYoursAccessKey.Description() ?? "";

            var emailMessage = GenerateEmailMessage
                (
                    message: message,
                    details: @$"
                        <p style='font-size: 18px; color: #666666;'>{yoursAccessKeyMessage}</p>

                        <p style='font-size: 18px; color: #666666;'>{acessKey}</p>"
                );

            _service.Send(to: to, subject: subject, message: emailMessage);
        }

        private static string GenerateEmailMessage(string message, string details)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                    <body>
                        <div class='email-container' style='width: 100%; max-width: 600px; margin: 0 auto; background-color: #ffffff; box-shadow: 0px 0px 10px 0px rgba(0,0,0,0.1); padding: 20px; border-radius: 8px;'>
                            <div style='text-align: center; padding: 10px 0;'>
                                <h1 style='color: #333333;'>{message}</h1>
                            </div>

                            <div style='text-align: center; padding: 20px 0;'>
                                {details}
                            </div>

                            <div style='text-align: center; margin-top: 30px; font-size: 12px; color: #999999'>
                                <p>© 2025 Autenticação em Dois Fatores. Todos os direitos reservados.</p>
                            </div>
                        </div>
                    </body>
                </html>";
        }
    }
}
