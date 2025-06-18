using AutenticacaoDoisFatores.Service.Dtos.Clients;

namespace AutenticacaoDoisFatores.Service.Builders
{
    public class NewClientBuilder
    {
        private string _name = "";
        private string _email = "";
        private string _domainName = "";
        private string _adminPassword = "";

        public NewClientBuilder WithName(string name)
        {
            _name = name;

            return this;
        }

        public NewClientBuilder WithEmail(string email)
        {
            _email = email;

            return this;
        }

        public NewClientBuilder WithDomainName(string domainName)
        {
            _domainName = domainName;

            return this;
        }

        public NewClientBuilder WithAdminPassword(string adminPassword)
        {
            _adminPassword = adminPassword;

            return this;
        }

        public NewClient Build()
        {
            return new NewClient
                (
                    name: _name,
                    email: _email,
                    domainName: _domainName,
                    adminPassword: _adminPassword
                );
        }
    }
}
