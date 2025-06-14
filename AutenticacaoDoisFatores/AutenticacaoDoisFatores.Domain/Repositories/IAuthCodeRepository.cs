namespace AutenticacaoDoisFatores.Domain.Repositories
{
    public interface IAuthCodeRepository
    {
        public Task SaveAsync(Guid userId, string code);
        public Task<string?> GetCodeByUserIdAsync(Guid userId);
    }
}
