using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Service.Shared;
using AutenticacaoDoisFatores.Domain.Builders;

namespace AutenticacaoDoisFatores.Service.Dtos.Clients
{
    public class NewClient
    {
        private readonly string _decryptedKey;

        public string Name { get; }
        public string Email { get; }
        public string DomainName { get; }
        public string AccessKey { get; }
        public string AdminPassword { get; }

        public NewClient(string name, string email, string domainName, string adminPassword)
        {
            Name = name;
            Email = email;
            DomainName = domainName;
            (_decryptedKey, AccessKey) = GenerateAccessKey();
            AdminPassword = adminPassword;
        }

        public string DecryptedKey()
        {
            return _decryptedKey;
        }

        private static (string descryptedKey, string encryptedKey) GenerateAccessKey()
        {
            return Security.GenerateEncryptedAuthCode();
        }

        public static explicit operator Client(NewClient newClient)
        {
            return new ClientBuilder()
                .WithName(newClient.Name)
                .WithEmail(newClient.Email)
                .WithDomainName(newClient.DomainName)
                .WithAccessKey(newClient.AccessKey)
                .BuildNew();
        }

        public static explicit operator User(NewClient newClient)
        {
            return new UserBuilder()
                .WithName(newClient.Name)
                .WithUsername("Administrador")
                .WithEmail(newClient.Email)
                .WithPassword(Encrypt.EncryptWithSha512(newClient.AdminPassword))
                .WithIsAdminFlag(true)
                .BuildNew();
        }
    }
}
