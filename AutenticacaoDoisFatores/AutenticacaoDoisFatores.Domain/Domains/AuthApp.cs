using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Exceptions;
using AutenticacaoDoisFatores.Domain.Services;

namespace AutenticacaoDoisFatores.Domain.Domains
{
    public partial class AuthApp(IAuthService service)
    {
        private readonly IAuthService _service = service;

        public string? GerarQrCode(User user)
        {
            if (user is null)
                UserException.UserNotFound();

            return _service.GenerateQrCode(user!.Email, user!.SecretKey);
        }

        public bool CodeIsValid(string code, User user)
        {
            if (code.IsNullOrEmptyOrWhiteSpaces())
                AuthAppException.CodeNotInformed();

            return _service.IsValidCode(code, user!.SecretKey);
        }
    }
}
