using AutenticacaoDoisFatores.Domain.Builders;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;

namespace AutenticacaoDoisFatores.Service.Dtos.Users
{
    public class NewUser(string name, string username, string email, string password, long? phone)
    {
        private readonly string _decryptedPassword = password;

        public string Name { get; } = name;
        public string Username { get; } = username;
        public string Email { get; } = email;
        public string Password { get; } = Encrypt.EncryptWithSha512(password);
        public long? Phone { get; } = phone;

        public string DecryptedPassword()
        {
            return _decryptedPassword;
        }

        public static explicit operator User(NewUser newUser)
        {
            return new UserBuilder()
                .WithName(newUser.Name)
                .WithUsername(newUser.Username)
                .WithEmail(newUser.Email)
                .WithPassword(newUser.Password)
                .WithPhone(newUser.Phone)
                .BuildNew();
        }
    }
}
