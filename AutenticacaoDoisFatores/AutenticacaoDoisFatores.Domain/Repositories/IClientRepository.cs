using AutenticacaoDoisFatores.Domain.Shared.Repositories;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Filters;

namespace AutenticacaoDoisFatores.Domain.Repositories
{
    public interface IClientRepository : IBaseRepository<Client, ClientFilter>
    {
        Task NewDomainAsync(string domainName);
        Task<bool> DomainExistsAsync(string domainName);
        Task<bool> EmailExistsAsync(string email);
        Task<Client?> GetByEmailAsync(string email);
        Task<string?> GetDomainNameByAccessKeyAsync(string accessKey);
    }
}
