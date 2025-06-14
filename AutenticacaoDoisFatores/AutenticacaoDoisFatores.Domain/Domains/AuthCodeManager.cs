using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Exceptions;
using AutenticacaoDoisFatores.Domain.Repositories;

namespace AutenticacaoDoisFatores.Domain.Domains
{
    public class AuthCodeManager(IAuthCodeRepository repository)
    {
        private readonly IAuthCodeRepository _repository = repository;

        public async Task SaveAsync(Guid userId, string code)
        {
            if (code.IsNullOrEmptyOrWhiteSpaces())
                AuthCodeException.EmptyAuthCode();

            await _repository.SaveAsync(userId, code);
        }

        public async Task<string?> GetCodeAsync(Guid userId)
        {
            return await _repository.GetCodeAsync(userId);
        }
    }
}
