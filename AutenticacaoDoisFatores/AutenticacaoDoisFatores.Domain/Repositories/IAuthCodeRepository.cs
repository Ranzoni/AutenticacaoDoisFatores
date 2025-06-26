namespace AutenticacaoDoisFatores.Domain.Repositories
{
    public interface IAuthCodeRepository
    {
        public Task SaveAsync(Guid userId, string code);
        public Task RemoveAsync(Guid userId);
        public Task<string?> GetCodeByUserIdAsync(Guid userId);
    }
}
