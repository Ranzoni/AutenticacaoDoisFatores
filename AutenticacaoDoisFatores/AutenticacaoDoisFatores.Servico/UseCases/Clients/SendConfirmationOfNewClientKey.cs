using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Service.Shared;
using Messenger;

namespace AutenticacaoDoisFatores.Service.UseCases.Clients
{
    public class SendConfirmationOfNewClientKey(ClientDomain domain, INotifier notifier, EmailSender email)
    {
        private readonly ClientDomain _domain = domain;
        private readonly INotifier _notifier = notifier;
        private readonly EmailSender _email = email;

        public async Task SendAsync(string email, string baseLinkConfirmation)
        {
            var client = await _domain.GetByEmailAsync(email);

            if (!IsValidEmail(client) || client is null)
                return;

            var newAccessKeyToken = Security.GenerateNewAccessKeyToken(client.Id);
            _email.SendNewAcessKeyGeneration(client.Email, baseLinkConfirmation, newAccessKeyToken);
        }

        private bool IsValidEmail(Client? client)
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
