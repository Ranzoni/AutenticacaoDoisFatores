using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Service.Shared;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Clients
{
    public class ResendClientKey(ClientDomain domain, INotifier notifier, EmailSender email)
    {
        private readonly ClientDomain _domain = domain;
        private readonly INotifier _notifier = notifier;
        private readonly EmailSender _email = email;

        public async Task ResendAsync(string email, string baseLinkToResend)
        {
            var client = await _domain.GetByEmailAsync(email);

            if (!IsValidResend(client) || client is null)
                return;

            var (accessKey, encryptedKey) = Security.GenerateEncryptedAuthCode();

            await UpdateAccessKeyAsync(client, encryptedKey);
            SendEmail(client.Id, client.Email, accessKey, baseLinkToResend);
        }

        private async Task UpdateAccessKeyAsync(Client client, string accessKey)
        {
            client.UpdateAccessKey(accessKey);
            await _domain.UpdateAsync(client);
        }

        private void SendEmail(Guid id, string email, string accessKey, string baseLinkConfirmation)
        {
            var confirmationToken = Security.GenerateClientConfirmationToken(id);
            _email.SendClientConfirmation(email, accessKey, baseLinkConfirmation, confirmationToken);
        }

        private bool IsValidResend(Client? client)
        {
            if (client is null)
            {
                _notifier.AddNotFoundMessage(ClientValidationMessages.ClientNotFound);
                return false;
            }

            if (client.Active)
                _notifier.AddMessage(ClientValidationMessages.ClientAlreadyActivated);

            return !_notifier.AnyMessage();
        }
    }
}
