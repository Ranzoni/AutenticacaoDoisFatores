using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Exceptions;
using AutenticacaoDoisFatores.Domain.Services;
using OtpNet;

namespace AutenticacaoDoisFatores.Infra.Services
{
    public class AuthService : IAuthService
    {
        private readonly string _app = "";

        public AuthService()
        {
            var app = Environment.GetEnvironmentVariable("ADF_CHAVE_APP_AUTENTICADOR");
            if (app is null || app.IsNullOrEmptyOrWhiteSpaces())
            {
                AuthAppException.SecretKeyNotFound();
                return;
            }

            _app = app;
        }

        public string GenerateQrCode(string email, string secretKey)
        {
            return $"otpauth://totp/{_app}:{email}?secret={secretKey}&issuer={_app}";
        }

        public bool IsValidCode(string code, string secretKey)
        {
            var bytes = Base32Encoding.ToBytes(secretKey);
            var totp = new Totp(bytes);
            return totp.VerifyTotp(code, out _);
        }

    }
}
