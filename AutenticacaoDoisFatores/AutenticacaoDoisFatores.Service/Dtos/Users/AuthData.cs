namespace AutenticacaoDoisFatores.Service.Dtos.Users
{
    public class AuthData(string usernameOrEmail, string password)
    {
        public string UsernameOrEmail { get; } = usernameOrEmail;
        public string Password { get; } = password;
    }
}
