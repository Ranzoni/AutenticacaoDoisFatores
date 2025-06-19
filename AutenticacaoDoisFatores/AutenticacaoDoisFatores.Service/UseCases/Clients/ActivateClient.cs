using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Clients
{
    public class ActivateClient(ClientDomain domain, INotifier notifier)
    {
        private readonly ClientDomain _domain = domain;
        private readonly INotifier _notifier = notifier;

        public async Task ActivateAsync(Guid clientId)
        {
            var client = await _domain.GetByIdAsync(clientId);
            if (!IsValidActivation(client) || client is null)
                return;

            client.SetActivate(true);
            await _domain.UpdateAsync(client);
        }

        private bool IsValidActivation(Client? client)
        {
            if (client is null)
                _notifier.AddNotFoundMessage(ClientValidationMessages.ClientNotFound);

            if (client?.Active ?? false)
                _notifier.AddMessage(ClientValidationMessages.ClientAlreadyActivated);

            return !_notifier.AnyMessage();
        }
    }
}
