using AutenticacaoDoisFatores.Domain.Shared.Permissions;

namespace AutenticacaoDoisFatores.Domain.Repositories
{
    public interface IPermissionsRepository
    {
        Task AddAsync(Guid userId, IEnumerable<PermissionType> permissions);
        Task<IEnumerable<PermissionType>> GetByUserIdAsync(Guid userId);
        Task UpdateAsync(Guid userId, IEnumerable<PermissionType> permissions);
    }
}
