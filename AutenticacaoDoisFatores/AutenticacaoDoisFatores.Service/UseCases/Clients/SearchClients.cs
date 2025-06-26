using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Filters;
using AutenticacaoDoisFatores.Service.Dtos.Clients;

namespace AutenticacaoDoisFatores.Service.UseCases.Clients
{
    public class SearchClients(ClientDomain domain)
    {
        private readonly ClientDomain _domain = domain;

        public async Task<RegisteredClient?> GetByIdAsync(Guid id)
        {
            var client = await _domain.GetByIdAsync(id);
            if (client is null)
                return null;

            return (RegisteredClient?)client;
        }

        public async Task<IEnumerable<RegisteredClient>> GetAllAsync(SearchClientsFilter searchFilter)
        {
            var filter = (ClientFilter)searchFilter;

            var clients = await _domain.GetAllAsync(filter);
            var registeredClients = clients.Select(c => (RegisteredClient)c);

            return registeredClients;
        }
    }
}
