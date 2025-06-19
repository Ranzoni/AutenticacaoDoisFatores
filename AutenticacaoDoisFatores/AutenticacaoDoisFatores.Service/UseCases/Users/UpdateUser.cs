using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Validators;
using AutenticacaoDoisFatores.Service.Dtos.Users;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Users
{
    public class UpdateUser(UserDomain domain, INotifier notifier)
    {
        private readonly UserDomain _domain = domain;
        private readonly INotifier _notifier = notifier;

        public async Task<RegisteredUser?> ExecuteAsync(Guid id, NewUserData newUserData)
        {
            var user = await _domain.GetByIdAsync(id);
            if (!await IsUpdateValidAsync(id, user, newUserData) || user is null)
                return null;

            if (!user.Name.Equals(newUserData.Name))
                user.UpdateName(newUserData.Name);

            if (!user.Username.Equals(newUserData.Username))
                user.UpdateUsername(newUserData.Username);

            if (!user.Phone.Equals(newUserData.Phone))
                user.UpdatePhone(newUserData.Phone);

            if (!user.AuthType.Equals(newUserData.AuthType))
                user.ConfigureAuthType(newUserData.AuthType);

            await _domain.UpdateAsync(user);

            return (RegisteredUser)user;
        }

        private async Task<bool> IsUpdateValidAsync(Guid id, User? user, NewUserData newUserData)
        {
            Task<bool>? checkIfUsernameAlreadyExists = null;
            Task<bool>? checkIfEmailAlreadyExists = null;

            if (user is null || !user.Active || user.IsAdmin)
                _notifier.AddNotFoundMessage(UserValidationMessages.UserNotFound);

            if (!UserValidator.IsNameValid(newUserData.Name))
                _notifier.AddMessage(UserValidationMessages.InvalidName);

            if (!UserValidator.IsUsernameValid(newUserData.Username))
                _notifier.AddMessage(UserValidationMessages.InvalidUsername);
            else 
                checkIfUsernameAlreadyExists = _domain.UsernameExistsAsync(newUserData.Username, id);

            if (!UserValidator.IsPhoneValid(newUserData.Phone))
                _notifier.AddMessage(UserValidationMessages.InvalidPhone);

            if (checkIfUsernameAlreadyExists is not null)
            {
                var usernameExists = await checkIfUsernameAlreadyExists;
                if (usernameExists)
                    _notifier.AddMessage(UserValidationMessages.UsernameAlreadyRegistered);
            }

            if (checkIfEmailAlreadyExists is not null)
            {
                var emailExists = await checkIfEmailAlreadyExists;
                if (emailExists)
                    _notifier.AddMessage(UserValidationMessages.EmailAlreadyRegistered);
            }

            return !_notifier.AnyMessage();
        }
    }
}
