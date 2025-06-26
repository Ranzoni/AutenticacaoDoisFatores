using AutenticacaoDoisFatores.Domain.Shared.Entities;
using AutenticacaoDoisFatores.Domain.Shared.Users;
using AutenticacaoDoisFatores.Domain.Entities;

namespace AutenticacaoDoisFatores.Service.Dtos.Users
{
    public class RegisteredUser : AuditedEntity
    {
        public string Name { get; }
        public string Username { get; }
        public string Email { get; }
        public long? Phone { get; }
        public bool Active { get; }
        public DateTime? LastAccess { get; }
        public AuthType? AuthType { get; }

        public RegisteredUser(Guid id, string name, string username, string email, long? phone, bool active, DateTime? lastAccess, AuthType? authType, DateTime createdAt, DateTime? updatedAt)
        {
            Id = id;
            Name = name;
            Username = username;
            Email = email;
            Phone = phone;
            Active = active;
            LastAccess = lastAccess;
            AuthType = authType;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    
        public static explicit operator RegisteredUser(User user)
        {
            return new RegisteredUser
            (
                id: user.Id,
                name: user.Name,
                username: user.Username,
                email: user.Email,
                phone: user.Phone,
                active: user.Active,
                lastAccess: user.LastAccess,
                authType: user.AuthType,
                createdAt: user.CreatedAt,
                updatedAt: user.UpdatedAt
            );
        }
    }
}
