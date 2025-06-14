using AutenticacaoDoisFatores.Domain.Shared.Permissions;

namespace AutenticacaoDoisFatores.Infra.Entities
{
    internal class Permission
    {
        internal Guid Id { get; private set; } = Guid.NewGuid();

        internal Guid UserId { get; private set; }

        internal IEnumerable<PermissionType> Permissions { get; private set; } = [];

        internal DateTime CreatedAt { get; private set; }

        internal DateTime? UpdatedAt { get; private set; }

        internal Permission(Guid userId, IEnumerable<PermissionType> permissions)
        {
            UserId = userId;
            Permissions = permissions;
            CreatedAt = DateTime.Now;
        }
    }
}
