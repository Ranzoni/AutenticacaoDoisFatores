using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Service.Dtos.Users;
using Messenger;
using AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators;
using Microsoft.Extensions.DependencyInjection;
using AutenticacaoDoisFatores.Domain.Validators;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios
{
    public class AuthenticateUser(INotifier notificador, UserDomain domain, IServiceProvider serviceProvider)
    {
        private readonly INotifier _notifier = notificador;
        private readonly UserDomain _domain = domain;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task<object?> ExecuteAsync(AuthData authData)
        {
            if (!IsAuthDataValid(authData))
                return null;

            var user = await GetUserAsync(authData);
            if (!AutenticacaoEhValida(user, authData.Password))
                return null;

            var authenticator = RetornarTipoDeAutenticador(user!);
            return await authenticator.ExecuteAsync(user!);
        }

        private bool IsAuthDataValid(AuthData authData)
        {
            var isUserNameEmpty = authData.UsernameOrEmail is null || authData.UsernameOrEmail.IsNullOrEmptyOrWhiteSpaces();

            if (isUserNameEmpty)
            {
                _notifier.AddMessage(UserValidationMessages.UsernameOrEmailRequired);
                return false;
            }

            return true;
        }

        private async Task<User?> GetUserAsync(AuthData authData)
        {
            if (UserValidator.IsEmailValid(authData.UsernameOrEmail))
                return await _domain.GetByEmailAsync(authData.UsernameOrEmail);
            else 
                return await _domain.GetByUsernameAsync(authData.UsernameOrEmail);
        }

        private bool AutenticacaoEhValida(User? user, string password)
        {
            if (user is null || !user.Active)
            {
                _notifier.AddUnauthorizedMessage(UserValidationMessages.Unauthorized);
                return false;
            }

            var senhaCriptografada = Encrypt.EncryptWithSha512(password);
            if (!user.Password.Equals(senhaCriptografada))
            {
                _notifier.AddUnauthorizedMessage(UserValidationMessages.Unauthorized);
                return false;
            }

            return true;
        }

        private IAuthType RetornarTipoDeAutenticador(User user)
        {
            if (user.AnyAuthTypeConfigured())
                return _serviceProvider.GetRequiredService<UserTwoFactorAuthentication>();
            else
                return _serviceProvider.GetRequiredService<BaseUserAuthenticator>();
        }
    }
}
