using AutenticacaoDoisFatores.Domain.Shared.Repositories;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Filters;

namespace AutenticacaoDoisFatores.Domain.Repositories
{
    public interface IUserRepository : IBaseRepository<User, UserFilter>
    {
        Task<bool> UsernameExistsAsync(string username, Guid? id = null);
        Task<bool> UsernameExistsAsync(string username, string domainName);
        Task<bool> EmailExistsAsync(string email, Guid? id = null);
        Task<bool> EmailExistsAsync(string email, string domainName);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        void Add(User user, string domainName);
        Task<bool> IsAdminAsync(Guid id);
    }
}
