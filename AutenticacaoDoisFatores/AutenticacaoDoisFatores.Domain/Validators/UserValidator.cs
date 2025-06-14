using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Exceptions;

namespace AutenticacaoDoisFatores.Domain.Validators
{
    public static class UserValidator
    {
        internal static void Validate(this User user)
        {
            if (!NameIsValid(user.Name))
                UserException.InvalidName();

            if (!UsernameIsValid(user.Username))
                UserException.InvalidUsername();

            if (!EmailIsValid(user.Email))
                UserException.InvalidEmail();

            if (!PasswordIsValid(user.Password))
                UserException.InvalidPassword();

            if (!PhoneIsValid(user.Phone))
                UserException.InvalidPhone();

            if (!SecretKeyIsValid(user.SecretKey))
                UserException.InvalidSecretKey();
        }

        public static bool NameIsValid(string name)
        {
            return !name.IsNullOrEmptyOrWhiteSpaces() && name.Length >= 3 && name.Length <= 50;
        }

        public static bool UsernameIsValid(string username)
        {
            return !username.IsNullOrEmptyOrWhiteSpaces() && username.Length >= 5 && username.Length <= 20;
        }

        public static bool EmailIsValid(string email)
        {
            return !email.IsNullOrEmptyOrWhiteSpaces() && email.IsValidEmail() && email.Length <= 256;
        }

        public static bool PasswordIsValid(string password)
        {
            return !password.IsNullOrEmptyOrWhiteSpaces() && password.Length <= 256;
        }

        public static bool PhoneIsValid(long? phone)
        {
            return phone is null || phone > 99999;
        }

        public static bool SecretKeyIsValid(string secretKey)
        {
            return !secretKey.IsNullOrEmptyOrWhiteSpaces() && secretKey.Length == 20;
        }
    }
}
