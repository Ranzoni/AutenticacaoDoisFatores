using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;

namespace AutenticacaoDoisFatores.Domain.Exceptions
{
    public class EmailException(EmailValidationMessages message) : ApplicationException(message.Description())
    {
        internal static void InvalidDestinationEmail()
        {
            throw new EmailException(EmailValidationMessages.InvalidDestinationEmail);
        }

        internal static void EmptySubject()
        {
            throw new EmailException(EmailValidationMessages.EmptySubject);
        }

        internal static void EmptyMessage()
        {
            throw new EmailException(EmailValidationMessages.EmptyMessage);
        }

        public static void SendingEmailNotFound()
        {
            throw new EmailException(EmailValidationMessages.SendingEmailNotFound);
        }
    }
}
