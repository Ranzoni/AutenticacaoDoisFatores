using AutenticacaoDoisFatores.Domain.Shared.Users;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators.TwoFactorAuths;
using Microsoft.Extensions.DependencyInjection;

namespace AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators
{
    public class UserTwoFactorAuthentication(IServiceProvider serviceProvider) : IAuthType
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task<object?> ExecuteAsync(User user)
        {
            IUserTwoFactorAuthType twoFactorAuthCode = user.AuthType switch
            {
                AuthType.Email => _serviceProvider.GetRequiredService<UserTwoFactorAuthByEmail>(),
                AuthType.AuthApp => _serviceProvider.GetRequiredService<UserTwoFactorAuthByApp>(),
                _ => throw new ApplicationException($"O tipo de autenticação de usuário é inválido. Tipo: {user.AuthType}.")
            };

            return await twoFactorAuthCode.SendAsync(user);
        }
    }
}
