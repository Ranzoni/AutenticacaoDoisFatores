using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Filters;
using AutenticacaoDoisFatores.Service.Dtos.Users;

namespace AutenticacaoDoisFatores.Service.UseCases.Users
{
    public class SearchUsers(UserDomain domain)
    {
        private readonly UserDomain _domain = domain;

        public async Task<RegisteredUser?> GetByIdAsync(Guid id)
        {
            var user = await _domain.GetByIdAsync(id);
            return (RegisteredUser?)user!;
        }

        public async Task<IEnumerable<RegisteredUser>> GetAllAsync(SearchUserFilters filterToSearch)
        {
            var userFilter = (UserFilter)filterToSearch;

            var users = await _domain.GetAllAsync(userFilter);
            var registeredUsers = users.Select(u => (RegisteredUser)u);
            return registeredUsers;
        }
    }
}
