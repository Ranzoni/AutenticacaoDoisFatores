using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Service.Dtos.Users;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Users
{
    public class SendEmailToUser(EmailSender email, UserDomain userDomain, INotifier notifier)
    {
        private readonly EmailSender _email = email;
        private readonly UserDomain _userDomain = userDomain;
        private readonly INotifier _notifier = notifier;

        public async Task ExecuteAsync(Guid userId, EmailSenderUser emailSenderUser)
        {
            var user = await _userDomain.GetByIdAsync(userId);
            if (!EnvioEhValido(user))
                return;

            _email.SendCustomizedEmailToUser(user!.Email, emailSenderUser.Subject, emailSenderUser.Message, emailSenderUser.HtmlEmail);
        }

        private bool EnvioEhValido(User? user)
        {
            if (user is null || user.IsAdmin)
            {
                _notifier.AddNotFoundMessage(UserValidationMessages.UserNotFound);
                return false;
            }

            return true;
        }
    }
}
