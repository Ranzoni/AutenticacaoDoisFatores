using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;

namespace AutenticacaoDoisFatores.Domain.Exceptions
{
    public class AuthAppException(AuthAppValidationMessages message) : ApplicationException(message.Description())
    {
        public static void SecretKeyNotFound()
        {
            throw new AuthAppException(AuthAppValidationMessages.SecretKeyNotFound);
        }

        public static void CodeNotInformed()
        {
            throw new AuthAppException(AuthAppValidationMessages.CodeNotInformed);
        }

        public static void QrCodeNotGenerated()
        {
            throw new AuthAppException(AuthAppValidationMessages.QrCodeNotGenerated);
        }
    }
}
