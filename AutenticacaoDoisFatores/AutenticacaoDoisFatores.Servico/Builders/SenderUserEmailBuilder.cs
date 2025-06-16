using AutenticacaoDoisFatores.Service.Dtos.Users;

namespace AutenticacaoDoisFatores.Service.Builders
{
    public class SenderUserEmailBuilder
    {
        private string _subject = "";
        private string _message = "";
        private string _htmlEmail = "";

        public SenderUserEmailBuilder WithSubject(string subject)
        {
            _subject = subject;

            return this;
        }

        public SenderUserEmailBuilder WithMessage(string message)
        {
            _message = message;

            return this;
        }

        public SenderUserEmailBuilder WithHtmlEmail(string htmlEmail)
        {
            _htmlEmail = htmlEmail;

            return this;
        }

        public EmailSenderUser Build()
        {
            return new EmailSenderUser
            (
                subject: _subject,
                message: _message,
                htmlEmail: _htmlEmail
            );
        }
    }
}
