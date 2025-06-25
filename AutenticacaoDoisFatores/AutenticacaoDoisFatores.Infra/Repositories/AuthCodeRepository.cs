using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Infra.Contexts;

namespace AutenticacaoDoisFatores.Infra.Repositories
{
    public class AuthCodeRepository(AuthCodeContext context) : IAuthCodeRepository
    {
        private readonly AuthCodeContext _context = context;

        public async Task<string?> GetCodeByUserIdAsync(Guid userId)
        {
            return await _context.GetByKeyAsync(UserIdKey(userId));
        }

        public async Task SaveAsync(Guid userId, string code)
        {
            await _context.SaveAsync(UserIdKey(userId), code);
        }

        private static string UserIdKey(Guid userId)
        {
            return $"userid_{userId}";
        }

        public async Task RemoveAsync(Guid userId)
        {
            await _context.RemoveAsync(UserIdKey(userId));
        }
    }
}
