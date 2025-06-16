using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Validators;
using AutenticacaoDoisFatores.Service.Dtos.Users;
using Messenger;
using AutenticacaoDoisFatores.Service.Shared;

namespace AutenticacaoDoisFatores.Service.UseCases.Users
{
    public class RegisterUser(INotifier notifier, UserDomain domain)
    {
        private readonly INotifier _notifier = notifier;
        private readonly UserDomain _domain = domain;

        public async Task<RegisteredUser?> RegisterAsync(NewUser newUser)
        {
            var isRegistrationValid = await IsRegistrationValid(newUser);
            if (!isRegistrationValid)
                return null;

            var user = (User)newUser;
            await _domain.CreateAsync(user);
            return (RegisteredUser)user;
        }

        private async Task<bool> IsRegistrationValid(NewUser newUser)
        {
            Task<bool>? checkIfUsernameAlreadyRegistered = null;
            Task<bool>? checkIfEmailAlreadyRegistered = null;

            if (!UserValidator.IsNameValid(newUser.Name))
                _notifier.AddMessage(UserValidationMessages.InvalidName);

            if (!UserValidator.IsUsernameValid(newUser.Username))
                _notifier.AddMessage(UserValidationMessages.InvalidUsername);
            else
                checkIfUsernameAlreadyRegistered = _domain.UsernameExistsAsync(newUser.Username);

            if (!UserValidator.IsEmailValid(newUser.Email))
                _notifier.AddMessage(UserValidationMessages.InvalidEmail);
            else
                checkIfEmailAlreadyRegistered = _domain.EmailExistsAsync(newUser.Email);

            if (!Security.IsPasswordValid(newUser.DecryptedPassword()))
                _notifier.AddMessage(UserValidationMessages.InvalidPassword);

            if (checkIfUsernameAlreadyRegistered is not null)
            {
                var usernameExists = await checkIfUsernameAlreadyRegistered;
                if (usernameExists)
                    _notifier.AddMessage(UserValidationMessages.UsernameAlreadyRegistered);
            }

            if (checkIfEmailAlreadyRegistered is not null)
            {
                var existsEmail = await checkIfEmailAlreadyRegistered;
                if (existsEmail)
                    _notifier.AddMessage(UserValidationMessages.EmailAlreadyRegistered);
            }

            return !_notifier.AnyMessage();
        }
    }
}
