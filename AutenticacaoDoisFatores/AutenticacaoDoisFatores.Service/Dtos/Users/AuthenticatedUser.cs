namespace AutenticacaoDoisFatores.Service.Dtos.Users
{
    public class AuthenticatedUser(RegisteredUser user, string token)
    {
        public RegisteredUser User { get; } = user;
        public string Token { get; } = token;
    }
}
