using AutenticacaoDoisFatores.Domain.Shared.Entities;
using AutenticacaoDoisFatores.Domain.Validators;

namespace AutenticacaoDoisFatores.Domain.Entities
{
    public class Client : AuditedEntity
    {
        public string Name { get; private set; } = "";
        public string Email { get; private set; } = "";
        public string DomainName { get; private set; } = "";
        public string AccessKey { get; private set; } = "";
        public bool Active { get; private set; } = false;

        private Client() : base(true) { }

        public Client(string name, string email, string domainName, string accessKey)
        {
            Name = name;
            Email = email;
            DomainName = domainName;
            AccessKey = accessKey;

            this.Validate();
        }

        public Client(Guid id, string name, string email, string domainName, string accessKey, bool active, DateTime createdAt, DateTime? updatedAt)
            : base(true)
        {
            Id = id;
            Name = name;
            Email = email;
            DomainName = domainName;
            AccessKey = accessKey;
            Active = active;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;

            this.Validate();
        }

        public void UpdateAccessKey(string accessKey)
        {
            RegisterUpdate(nameof(AccessKey), AccessKey, accessKey);

            AccessKey = accessKey;
        }

        public void SetActivate(bool value)
        {
            RegisterUpdate(nameof(Active), Active.ToString(), value.ToString());

            Active = value;
        }
    }
}
