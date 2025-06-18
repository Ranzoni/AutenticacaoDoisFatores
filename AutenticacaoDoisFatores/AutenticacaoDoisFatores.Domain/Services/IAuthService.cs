namespace AutenticacaoDoisFatores.Domain.Services
{
    public interface IAuthService
    {
        string GenerateQrCode(string email, string secretKey);
        bool IsCodeValid(string code, string secretKey);
    }
}
