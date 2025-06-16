using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Service.Dtos.Users;

namespace AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators.TwoFactorAuths
{
    internal interface IUserTwoFactorAuthType
    {
        Task<TwoFactorAuthResponse?> SendAsync(User user);
    }
}
