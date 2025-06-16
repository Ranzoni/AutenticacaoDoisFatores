using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Exceptions;

namespace AutenticacaoDoisFatores.Domain.Validators
{
    public static class UserValidator
    {
        internal static void Validate(this User user)
        {
            if (!IsNameValid(user.Name))
                UserException.InvalidName();

            if (!IsUsernameValid(user.Username))
                UserException.InvalidUsername();

            if (!IsEmailValid(user.Email))
                UserException.InvalidEmail();

            if (!IsPasswordValid(user.Password))
                UserException.InvalidPassword();

            if (!IsPhoneValid(user.Phone))
                UserException.InvalidPhone();

            if (!IsSecretKeyValid(user.SecretKey))
                UserException.InvalidSecretKey();
        }

        public static bool IsNameValid(string name)
        {
            return !name.IsNullOrEmptyOrWhiteSpaces() && name.Length >= 3 && name.Length <= 50;
        }

        public static bool IsUsernameValid(string username)
        {
            return !username.IsNullOrEmptyOrWhiteSpaces() && username.Length >= 5 && username.Length <= 20;
        }

        public static bool IsEmailValid(string email)
        {
            return !email.IsNullOrEmptyOrWhiteSpaces() && email.IsValidEmail() && email.Length <= 256;
        }

        public static bool IsPasswordValid(string password)
        {
            return !password.IsNullOrEmptyOrWhiteSpaces() && password.Length <= 256;
        }

        public static bool IsPhoneValid(long? phone)
        {
            return phone is null || phone > 99999;
        }

        public static bool IsSecretKeyValid(string secretKey)
        {
            return !secretKey.IsNullOrEmptyOrWhiteSpaces() && secretKey.Length == 20;
        }
    }
}
