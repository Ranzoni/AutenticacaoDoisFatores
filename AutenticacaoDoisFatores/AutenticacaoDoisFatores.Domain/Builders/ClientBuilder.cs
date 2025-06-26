using AutenticacaoDoisFatores.Domain.Entities;

namespace AutenticacaoDoisFatores.Domain.Builders
{
    public class ClientBuilder
    {
        private Guid _id;
        private string _name = "";
        private string _email = "";
        private string _domainName = "";
        private string _accessKey = "";
        private bool _active;
        private DateTime _createdAt;
        private DateTime? _updatedAt;

        public ClientBuilder WithId(Guid id)
        {
            _id = id;

            return this;
        }

        public ClientBuilder WithName(string name)
        {
            _name = name;

            return this;
        }

        public ClientBuilder WithEmail(string email)
        {
            _email = email;

            return this;
        }

        public ClientBuilder WithDomainName(string domainName)
        {
            _domainName = domainName;

            return this;
        }

        public ClientBuilder WithAccessKey(string accessKey)
        {
            _accessKey = accessKey;

            return this;
        }

        public ClientBuilder WithActive(bool active)
        {
            _active = active;

            return this;
        }

        public ClientBuilder WithCreatedAt(DateTime createdAt)
        {
            _createdAt = createdAt;

            return this;
        }

        public ClientBuilder WithUpdatedAt(DateTime? updatedAt)
        {
            _updatedAt = updatedAt;

            return this;
        }

        public Client BuildNew()
        {
            var client = new Client(name: _name, email: _email, domainName: _domainName, accessKey: _accessKey);

            return client;
        }

        public Client Build()
        {
            var client = new Client
            (
                id: _id,
                name: _name,
                email: _email,
                domainName: _domainName,
                accessKey: _accessKey,
                active: _active,
                createdAt: _createdAt,
                updatedAt: _updatedAt
            );

            return client;
        }
    }
}
