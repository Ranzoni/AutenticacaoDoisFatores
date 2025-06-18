namespace AutenticacaoDoisFatores.Service.Dtos.Users
{
    public class AuthCodeUser(string code)
    {
        public string Code { get; } = code;
    }
}
