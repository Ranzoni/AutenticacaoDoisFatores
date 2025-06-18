namespace AutenticacaoDoisFatores.Service.Dtos.Users
{
    public class TwoFactorAuthResponse(string token)
    {
        public string Token { get; } = token;
    }
}
