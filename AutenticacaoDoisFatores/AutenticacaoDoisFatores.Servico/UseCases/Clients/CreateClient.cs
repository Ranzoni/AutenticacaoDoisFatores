using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Validators;
using AutenticacaoDoisFatores.Service.Dtos.Clients;
using AutenticacaoDoisFatores.Service.Exceptions;
using Messenger;
using AutenticacaoDoisFatores.Service.Shared;

namespace AutenticacaoDoisFatores.Service.UseCases.Clients
{
    public class CreateClient(ClientDomain domain, UserDomain userDomain, INotifier notifier, EmailSender email)
    {
        private readonly ClientDomain _domain = domain;
        private readonly UserDomain _userDomain = userDomain;
        private readonly INotifier _notifier = notifier;
        private readonly EmailSender _email = email;

        public async Task<RegisteredClient?> ExecuteAsync(NewClient newClient, string baseLinkConfirmation)
        {
            var isValid = await IsValidCreation(newClient, baseLinkConfirmation);
            if (!isValid)
                return null;

            var client = await RegisterClientAsync(newClient);
            await RegisterUserAdminAsync(newClient);

            SendEmail(client.Id, newClient, baseLinkConfirmation);

            return (RegisteredClient)client;
        }

        private async Task<bool> IsValidCreation(NewClient newClient, string confirmationLink)
        {
            if (!ClientValidator.IsNameValid(newClient.Name))
                _notifier.AddMessage(ClientValidationMessages.InvalidName);

            if (!ClientValidator.IsEmailValid(newClient.Email))
                _notifier.AddMessage(ClientValidationMessages.InvalidEmail);

            if (!ClientValidator.IsDomainNameValid(newClient.DomainName))
                _notifier.AddMessage(ClientValidationMessages.InvalidDomainName);

            if (!ClientValidator.IsAccesKeyValid(newClient.AccessKey))
                _notifier.AddMessage(ClientValidationMessages.InvalidAccessKey);

            if (!Security.IsPasswordValid(newClient.AdminPassword))
                _notifier.AddMessage(UserValidationMessages.InvalidPassword);

            if (confirmationLink.IsNullOrEmptyOrWhiteSpaces())
                ClientRegisterException.ConfirmationLinkNotInformed();

            var emailExists = await _domain.EmailExistsAsync(newClient.Email);
            if (emailExists)
                _notifier.AddMessage(ClientValidationMessages.EmailAlreadyRegistered);

            var domainNameExists = await _domain.DomainExistsAsync(newClient.DomainName);
            if (domainNameExists)
                _notifier.AddMessage(ClientValidationMessages.DomainNameAlreadyRegistered);

            return !_notifier.AnyMessage();
        }

        private async Task<Client> RegisterClientAsync(NewClient newClient)
        {
            var client = (Client)newClient;

            await _domain.CreateAsync(client);
            await _domain.CreateDomainAsync(client.Id);

            return client;
        }

        private async Task RegisterUserAdminAsync(NewClient newClient)
        {
            var userAdmin = (User)newClient;
            await _userDomain.CreateByDomainAsync(userAdmin, newClient.DomainName);
        }

        private void SendEmail(Guid id, NewClient newClient, string baseLinkConfirmation)
        {
            var confirmationToken = Security.GenerateClientConfirmationToken(id);
            _email.SendClientConfirmation(newClient.Email, newClient.DescryptedKey(), baseLinkConfirmation, confirmationToken);
        }
    }
}
