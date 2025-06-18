using AutenticacaoDoisFatores.Domain.Shared.Users;
using AutenticacaoDoisFatores.Service.Dtos.Users;

namespace AutenticacaoDoisFatores.Service.Builders
{
    public class NewUserDataBuilder
    {
        private string _name = "";
        private string _username = "";
        private long? _phone;
        private AuthType? _authType;

        public NewUserDataBuilder WithName(string name)
        {
            _name = name;

            return this;
        }

        public NewUserDataBuilder WithUsername(string username)
        {
            _username = username;

            return this;
        }

        public NewUserDataBuilder WithPhone(long? phone)
        {
            _phone = phone;

            return this;
        }

        public NewUserDataBuilder ComTipoDeAutenticacao(AuthType? authType)
        {
            _authType = authType;

            return this;
        }

        public NewUserData Build()
        {
            return new NewUserData
                (
                    name: _name,
                    username: _username,
                    phone: _phone,
                    authType: _authType
                );
        }
    }
}
