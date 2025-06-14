using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Exceptions;

namespace AutenticacaoDoisFatores.Domain.Validators
{
    public static class ClientValidator
    {
        internal static void Validate(this Client client)
        {
            if (!NameIsValid(client.Name))
                ClientException.InvalidName();

            if (!EmailIsValid(client.Email))
                ClientException.InvalidEmail();

            if (!DomainNameIsValid(client.DomainName))
                ClientException.InvalidDomainName();

            if (!AccesKeyIsValid(client.AccessKey))
                ClientException.InvalidAccessKey();
        }

        public static bool NameIsValid(string name)
        {
            return !name.IsNullOrEmptyOrWhiteSpaces() && name.Length >= 3 && name.Length <= 50;
        }

        public static bool EmailIsValid(string email)
        {
            return !email.IsNullOrEmptyOrWhiteSpaces() && email.IsValidEmail() && email.Length <= 256;
        }

        public static bool DomainNameIsValid(string domainName)
        {
            return !domainName.IsNullOrEmptyOrWhiteSpaces()
                && domainName.Length >= 3
                && domainName.Length <= 15
                && !domainName.AnySpecialCharactersOrPontuations()
                && !domainName.Contains(' ');
        }

        public static bool AccesKeyIsValid(string accessKey)
        {
            return !accessKey.IsNullOrEmptyOrWhiteSpaces()
                && accessKey.Length >= 3
                && accessKey.Length <= 2000;
        }
    }
}
