using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;

namespace AutenticacaoDoisFatores.Domain.Exceptions
{
    public class ClientException(ClientValidationMessages message) : ApplicationException(message.Description())
    {
        internal static void InvalidName()
        {
            throw new ClientException(ClientValidationMessages.InvalidName);
        }

        internal static void InvalidEmail()
        {
            throw new ClientException(ClientValidationMessages.InvalidEmail);
        }

        internal static void InvalidDomainName()
        {
            throw new ClientException(ClientValidationMessages.InvalidDomainName);
        }

        internal static void InvalidAccessKey()
        {
            throw new ClientException(ClientValidationMessages.InvalidAccessKey);
        }

        internal static void EmailAlreadyRegistered()
        {
            throw new ClientException(ClientValidationMessages.EmailAlreadyRegistered);
        }

        internal static void DomainNameAlreadyRegistered()
        {
            throw new ClientException(ClientValidationMessages.DomainNameAlreadyRegistered);
        }
    }
}
