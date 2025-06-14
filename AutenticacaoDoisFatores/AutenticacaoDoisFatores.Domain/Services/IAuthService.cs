namespace AutenticacaoDoisFatores.Domain.Services
{
    public interface IAuthService
    {
        string GenerateQrCode(string email, string secretKey);
        bool IsValidCode(string code, string secretKey);
    }
}
