using AutenticacaoDoisFatores.Domain.Shared.Permissions;
using AutenticacaoDoisFatores.Domain.Repositories;

namespace AutenticacaoDoisFatores.Domain.Domains
{
    public class PermissionsDomain(IPermissionRepository repository)
    {
        private readonly IPermissionRepository _repository = repository;

        public async Task AddAsync(Guid userId, IEnumerable<PermissionType> permissionsToInclude)
        {
            var currentPermissions = await _repository.GetByUserIdAsync(userId) ?? [];
            if (currentPermissions.Any())
            {
                var newPermissions = permissionsToInclude.Except(currentPermissions);
                var updatedPermissions = currentPermissions.Concat(newPermissions);
                await _repository.UpdateAsync(userId, updatedPermissions);
            }
            else
                await _repository.AddAsync(userId, permissionsToInclude);
        }

        public async Task RemoverAsync(Guid userId, IEnumerable<PermissionType> permissionsToRemove)
        {
            var currentPermissions = await GetByIdAsync(userId) ?? [];
            if (!currentPermissions.Any())
                return;

            var updatedPermissions = currentPermissions.Except(permissionsToRemove);

            await _repository.UpdateAsync(userId, updatedPermissions);
        }

        public async Task<IEnumerable<PermissionType>> GetByIdAsync(Guid userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }

        public static IEnumerable<PermissionType> GetAll()
        {
            return Enum.GetValues<PermissionType>();
        }
    }
}
