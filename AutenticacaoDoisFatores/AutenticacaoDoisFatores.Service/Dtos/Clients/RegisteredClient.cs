using AutenticacaoDoisFatores.Domain.Shared.Entities;
using AutenticacaoDoisFatores.Domain.Entities;

namespace AutenticacaoDoisFatores.Service.Dtos.Clients
{
    public class RegisteredClient : AuditedEntity
    {
        public string Name { get; }
        public string Email { get; }
        public string DomainName { get; }
        public bool Active { get; }

        public RegisteredClient(Guid id, string name, string email, string domainName, bool active, DateTime createdAt, DateTime? updatedAt)
        {
            Id = id;
            Name = name;
            Email = email;
            DomainName = domainName;
            Active = active;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public static explicit operator RegisteredClient(Client client)
        {
            return new RegisteredClient
            (
                id: client.Id,
                name: client.Name,
                email: client.Email,
                domainName: client.DomainName,
                active: client.Active,
                createdAt: client.CreatedAt,
                updatedAt: client.UpdatedAt
            );
        }
    }
}
