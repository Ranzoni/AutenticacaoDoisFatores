using AutenticacaoDoisFatores.Domain.Shared.Users;

namespace AutenticacaoDoisFatores.Service.Dtos.Users
{
    public class NewUserData(string name, string username, long? phone, AuthType? authType)
    {
        public string Name { get; } = name;
        public string Username { get; } = username;
        public long? Phone { get; } = phone;
        public AuthType? AuthType { get; } = authType;
    }
}
