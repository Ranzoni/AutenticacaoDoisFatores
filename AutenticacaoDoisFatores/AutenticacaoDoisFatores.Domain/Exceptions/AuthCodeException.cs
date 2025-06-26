using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;

namespace AutenticacaoDoisFatores.Domain.Exceptions
{
    public class AuthCodeException(AuthCodeValidationMessages message) : ApplicationException(message.Description())
    {
        internal static void EmptyAuthCode()
        {
            throw new AuthCodeException(AuthCodeValidationMessages.EmptyAuthCode);
        }
    }
}
