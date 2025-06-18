using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Service.Shared;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Clients
{
    public class GenerateClientAccessKey(ClientDomain domain, INotifier notifier, EmailSender email)
    {
        private readonly ClientDomain _domain = domain;
        private readonly INotifier _notifier = notifier;
        private readonly EmailSender _email = email;

        public async Task GenerateAsync(Guid clientId)
        {
            var client = await _domain.GetByIdAsync(clientId);

            if (!IsValidGeneration(client) || client is null)
                return;

            var (key, encryptedKey) = Security.GenerateEncryptedAuthCode();

            client.UpdateAccessKey(encryptedKey);
            await _domain.UpdateAsync(client);

            _email.SendNewAccessKeyConfirmation(client.Email, key);
        }

        private bool IsValidGeneration(Client? client)
        {
            if (client is null)
            {
                _notifier.AddNotFoundMessage(ClientValidationMessages.ClientNotFound);
                return false;
            }

            if (!client.Active)
                _notifier.AddMessage(ClientValidationMessages.ClientNotActive);

            return !_notifier.AnyMessage();
        }
    }
}
