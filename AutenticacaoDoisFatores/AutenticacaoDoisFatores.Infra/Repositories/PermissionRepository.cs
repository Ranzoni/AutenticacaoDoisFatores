using AutenticacaoDoisFatores.Domain.Shared.Permissions;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Infra.Contexts;
using AutenticacaoDoisFatores.Infra.Entities;
using AutenticacaoDoisFatores.Infra.Utils;
using System.Linq.Expressions;

namespace AutenticacaoDoisFatores.Infra.Repositories
{
    public class PermissionRepository(PermissionsContext context) : IPermissionRepository
    {
        private readonly PermissionsContext _context = context;

        public async Task AddAsync(Guid userId, IEnumerable<PermissionType> permissions)
        {
            var permission = new Permission(userId, permissions);

            await _context.Permissions.AddAsync(permission);

            var audit = _context.BuildAudit(typeof(Permission), userId, AuditActions.Inclusion, permission);
            if (audit is not null)
                await _context.Audits.AddAsync(audit);
        }

        public async Task<IEnumerable<PermissionType>> GetByUserIdAsync(Guid userId)
        {
            var permission = await _context.Permissions
                .GetAsync(p => p.UserId.Equals(userId));

            return permission?.Permissions ?? [];
        }

        public async Task UpdateAsync(Guid userId, IEnumerable<PermissionType> permissions)
        {
            var fieldsToUpdate = new Dictionary<Expression<Func<Permission, object>>, object>
            {
                { p => p.Permissions, permissions },
                { p => p.UpdatedAt, DateTime.UtcNow }
            };

            await _context.Permissions.UpdateAsync
            (
                expressionFilter: p => p.UserId == userId,
                fieldsToUpdate: fieldsToUpdate
            );

            var audit = _context.BuildAudit(typeof(Permission), userId, AuditActions.Change, new { permissions });
            if (audit is not null)
                await _context.Audits.AddAsync(audit);
        }
    }
}
