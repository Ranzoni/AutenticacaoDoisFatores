using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Entities;
using AutenticacaoDoisFatores.Domain.Shared.Users;
using AutenticacaoDoisFatores.Domain.Validators;

namespace AutenticacaoDoisFatores.Domain.Entities
{
    public class User : AuditedEntity
    {
        public string Name { get; private set; } = "";
        public string Username { get; private set; } = "";
        public string Email { get; private set; } = "";
        public string Password { get; private set; } = "";
        public long? Phone { get; private set; }
        public bool Active { get; private set; }
        public DateTime? LastAccess { get; private set; }
        public bool IsAdmin { get; private set; }
        public AuthType? AuthType { get; private set; }
        public string SecretKey { get; }
        public string LastDataChange { get; private set; }

        public User(string name, string username, string email, string password, long? phone, bool isAdmin = false)
        {
            Name = name;
            Username = username;
            Email = email;
            Password = password;
            Phone = phone;
            SecretKey = Secrets.Generate();
            LastDataChange = CreatedAt.Ticks.ToString();

            if (isAdmin)
            {
                Active = true;
                IsAdmin = true;
            }

            this.Validate();
        }

        public User(Guid id, string name, string username, string email, string password, long? phone, bool active, DateTime? lastAccess, DateTime createdAt, DateTime? updatedAt, bool isAdmin, AuthType? authType, string secretKey, string lastDataChange)
            : base(true)
        {
            Id = id;
            Name = name;
            Username = username;
            Email = email;
            Password = password;
            Phone = phone;
            Active = active;
            LastAccess = lastAccess;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsAdmin = isAdmin;
            AuthType = authType;
            SecretKey = secretKey;
            LastDataChange = lastDataChange;

            this.Validate();
        }

        public void UpdateName(string name)
        {
            RegisterUpdate(nameof(Name), Name, name);

            Name = name;

            this.Validate();
        }

        public void UpdateUsername(string username)
        {
            RegisterUpdate(nameof(Username), Username, username);

            Username = username;

            this.Validate();
        }

        public void UpdateEmail(string email)
        {
            RegisterUpdate(nameof(Email), Email, email);

            Email = email;
            LastDataChange = UpdatedAt!.Value.Ticks.ToString();

            this.Validate();
        }

        public void SetActive(bool value)
        {
            RegisterUpdate(nameof(Active), Active.ToString(), value.ToString());

            Active = value;
        }

        public void UpdatePassword(string password)
        {
            RegisterUpdate(nameof(Password), Password, password);

            Password = password;
            LastDataChange = UpdatedAt!.Value.Ticks.ToString();

            this.Validate();
        }

        public void UpdatePhone(long? phone)
        {
            RegisterUpdate(nameof(Phone), Phone?.ToString() ?? "", phone?.ToString() ?? "");

            Phone = phone;

            this.Validate();
        }

        public void UpdateLastAccess()
        {
            var newDate = DateTime.Now;

            RegisterUpdate(nameof(LastAccess), $"{LastAccess:yyyy-MM-dd HH:mm:ss}", $"{newDate:yyyy-MM-dd HH:mm:ss}");

            LastAccess = newDate;
        }

        public bool AnyAuthTypeConfigured()
        {
            return AuthType is not null;
        }

        public void ConfigureAuthType(AuthType? authType)
        {
            RegisterUpdate(nameof(AuthType), AuthType?.ToString() ?? "", authType.ToString() ?? "");

            AuthType = authType;
        }
    }
}
