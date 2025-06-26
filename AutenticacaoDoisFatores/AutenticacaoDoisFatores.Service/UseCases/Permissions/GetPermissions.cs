using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Service.Dtos.Permissions;

namespace AutenticacaoDoisFatores.Service.UseCases.Permissions
{
    public class GetPermissions(PermissionsDomain domain, UserDomain userDomain)
    {
        private readonly PermissionsDomain _domain = domain;
        private readonly UserDomain _userDomain = userDomain;

        public static IEnumerable<AvaiblePermission> GetAll()
        {
            var permissions = PermissionsDomain.GetAll()
                .Select(type => new AvaiblePermission(type.Description() ?? "", type));

            return permissions;
        }

        public async Task<IEnumerable<AvaiblePermission>> GetByUserIdAsync(Guid userId)
        {
            if (await _userDomain.IsAdminAsync(userId))
                return GetAll();

            var permissions = await _domain.GetByUserIdAsync(userId);

            return permissions
                .Select(type => new AvaiblePermission(type.Description() ?? "", type));
        }
    }
}
