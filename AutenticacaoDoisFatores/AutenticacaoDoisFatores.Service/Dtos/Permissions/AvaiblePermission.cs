using AutenticacaoDoisFatores.Domain.Shared.Permissions;

namespace AutenticacaoDoisFatores.Service.Dtos.Permissions
{
    public class AvaiblePermission(string name, PermissionType value)
    {
        public string Name { get; } = name;
        public PermissionType Value { get; } = value;
    }
}
