using AutenticacaoDoisFatores.Domain.Shared.Messages;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Exceptions;
using AutenticacaoDoisFatores.Domain.Filters;
using AutenticacaoDoisFatores.Domain.Repositories;

namespace AutenticacaoDoisFatores.Domain.Domains
{
    public partial class ClientDomain(IClientRepository repository)
    {
        private readonly IClientRepository _repository = repository;

        #region Write

        public async Task<Client> CreateAsync(Client client)
        {
            await ValidateCreationAsync(client);

            _repository.Add(client);
            await _repository.SaveChangesAsync();

            return client;
        }

        public async Task CreateDomainAsync(Guid clientId)
        {
            var client = await _repository.GetByIdAsync(clientId)
                ?? throw new ClientException(ClientValidationMessages.ClientNotFound);

            await _repository.NewDomainAsync(client.DomainName);
        }

        public async Task<Client> UpdateAsync(Client client)
        {
            _repository.Update(client);
            await _repository.SaveChangesAsync();

            return client;
        }

        #endregion Write

        #region Read

        public async Task<Client?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Client>> GetAllAsync(ClientFilter filter)
        {
            return await _repository.GetAllAsync(filter);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _repository.EmailExistsAsync(email);
        }

        public async Task<bool> DomainExistsAsync(string domainName)
        {
            return await _repository.DomainExistsAsync(domainName);
        }

        public async Task<Client?> GetByEmailAsync(string email)
        {
            return await _repository.GetByEmailAsync(email);
        }

        #endregion Read
    }

    #region Validator

    public partial class ClientDomain
    {
        private async Task ValidateCreationAsync(Client client)
        {
            var emailExists = await _repository.EmailExistsAsync(client.Email);
            if (emailExists)
                ClientException.EmailAlreadyRegistered();

            var domainExists = await _repository.DomainExistsAsync(client.DomainName);
            if (domainExists)
                ClientException.DomainNameAlreadyRegistered();
        }
    }

    #endregion
}
