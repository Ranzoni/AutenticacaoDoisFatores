using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;

namespace AutenticacaoDoisFatores.Domain.Exceptions
{
    public class UserException(UserValidationMessages message) : ApplicationException(message.Description())
    {
        internal static void InvalidName()
        {
            throw new UserException(UserValidationMessages.InvalidName);
        }

        internal static void InvalidUsername()
        {
            throw new UserException(UserValidationMessages.InvalidUsername);
        }

        internal static void InvalidEmail()
        {
            throw new UserException(UserValidationMessages.InvalidEmail);
        }

        internal static void InvalidPassword()
        {
            throw new UserException(UserValidationMessages.InvalidPassword);
        }

        internal static void InvalidPhone()
        {
            throw new UserException(UserValidationMessages.InvalidPhone);
        }

        internal static void UsernameAlreadyRegistered()
        {
            throw new UserException(UserValidationMessages.UsernameAlreadyRegistered);
        }

        internal static void EmailAlreadyRegistered()
        {
            throw new UserException(UserValidationMessages.EmailAlreadyRegistered);
        }

        internal static void UserNotFound()
        {
            throw new UserException(UserValidationMessages.UserNotFound);
        }

        internal static void InvalidSecretKey()
        {
            throw new UserException(UserValidationMessages.InvalidSecretKey);
        }
    }
}
