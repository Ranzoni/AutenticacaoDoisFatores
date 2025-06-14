using AutenticacaoDoisFatores.Domain.Shared.Users;
using AutenticacaoDoisFatores.Domain.Entities;

namespace AutenticacaoDoisFatores.Domain.Builders
{
    public class UserBuilder
    {
        private Guid _id;
        private string _name = "";
        private string _userName = "";
        private string _email = "";
        private string _password = "";
        private long? _phone;
        private bool _active = false;
        private bool _isAdmin = false;
        private DateTime? _lastAccess;
        private DateTime _createdAt;
        private DateTime? _updatedAt;
        private AuthType? _authType;
        private string _secretKey = "01234567890123456789";

        public UserBuilder WithId(Guid id)
        {
            _id = id;

            return this;
        }

        public UserBuilder WithName(string name)
        {
            _name = name;

            return this;
        }

        public UserBuilder WithUserName(string userName)
        {
            _userName = userName;

            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            _email = email;

            return this;
        }

        public UserBuilder WithPassword(string password)
        {
            _password = password;

            return this;
        }

        public UserBuilder WithPhone(long? phone)
        {
            _phone = phone;

            return this;
        }

        public UserBuilder WithActive(bool active)
        {
            _active = active;

            return this;
        }

        public UserBuilder WithIsAdminFlag(bool value)
        {
            _isAdmin = value;

            return this;
        }

        public UserBuilder WithAuthType(AuthType? authType)
        {
            _authType = authType;

            return this;
        }

        public UserBuilder WithLastAccess(DateTime? lastAccess)
        {
            _lastAccess = lastAccess;

            return this;
        }

        public UserBuilder WithCreatedAt(DateTime createdAt)
        {
            _createdAt = createdAt;

            return this;
        }

        public UserBuilder WithUpdatedAt(DateTime? updatedAt)
        {
            _updatedAt = updatedAt;

            return this;
        }

        public UserBuilder WithSecretKey(string secretKey)
        {
            _secretKey = secretKey;

            return this;
        }

        public User BuildNew()
        {
            var user = new User
            (
                name: _name,
                username: _userName,
                email: _email,
                password: _password,
                phone: _phone,
                isAdmin: _isAdmin
            );

            return user;
        }

        public User Build()
        {
            var user = new User
            (
                id: _id,
                name: _name,
                username: _userName,
                email: _email,
                password: _password,
                phone: _phone,
                active: _active,
                lastAccess: _lastAccess,
                createdAt: _createdAt,
                updatedAt: _updatedAt,
                isAdmin: _isAdmin,
                authType: _authType,
                secretKey: _secretKey
            );

            return user;
        }
    }
}
