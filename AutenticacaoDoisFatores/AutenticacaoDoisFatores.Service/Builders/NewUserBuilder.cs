using AutenticacaoDoisFatores.Service.Dtos.Users;

namespace AutenticacaoDoisFatores.Service.Builders
{
    public class NewUserBuilder
    {
        private string _name = "";
        private string _email = "";
        private string _username = "";
        private string _password = "";
        private long? _phone;

        public NewUserBuilder WithName(string name)
        {
            _name = name;

            return this;
        }

        public NewUserBuilder ComUsername(string username)
        {
            _username = username;

            return this;
        }

        public NewUserBuilder WithEmail(string email)
        {
            _email = email;

            return this;
        }

        public NewUserBuilder WithPassword(string password)
        {
            _password = password;

            return this;
        }

        public NewUserBuilder WithPhone(long? phone)
        {
            _phone = phone;

            return this;
        }

        public NewUser Build()
        {
            return new NewUser
                (
                    name: _name,
                    username: _username,
                    email: _email,
                    password: _password,
                    phone: _phone
                );
        }
    }
}
