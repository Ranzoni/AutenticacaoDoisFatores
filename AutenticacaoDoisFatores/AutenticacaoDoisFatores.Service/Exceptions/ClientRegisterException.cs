using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;

namespace AutenticacaoDoisFatores.Service.Exceptions
{
    public class ClientRegisterException(ClientValidationMessages message) : ApplicationException(message.Description())
    {
        internal static void ConfirmationLinkNotInformed()
        {
            throw new ClientRegisterException(ClientValidationMessages.ConfirmationLinkNotInformed);
        }
    }
}
