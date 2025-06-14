using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Exceptions;
using AutenticacaoDoisFatores.Domain.Filters;
using AutenticacaoDoisFatores.Domain.Repositories;

namespace AutenticacaoDoisFatores.Domain.Domains
{
    public partial class UserDomain(IUserRepository repository)
    {
        private readonly IUserRepository _repository = repository;

        #region Write

        public async Task CreateAsync(User user)
        {
            await ValidateCreationAsync(user);

            _repository.Add(user);
            await _repository.SaveChangesAsync();
        }

        public async Task CreateByDomainAsync(User user, string domainName)
        {
            await ValidateCreationByDomainAsync(user, domainName);

            _repository.Add(user, domainName);
            await _repository.SaveChangesAsync();
        }

        public async Task<User> UpdateAsync(User user)
        {
            await ValidateChangeAsync(user);

            _repository.Update(user);
            await _repository.SaveChangesAsync();

            return user;
        }

        public async Task RemoveAsync(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);

            ValidateRemoval(user);

            _repository.Remove(user!);
            await _repository.SaveChangesAsync();
        }

        #endregion

        #region Read

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync(UserFilter filter)
        {
            return await _repository.GetAllAsync(filter);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _repository.GetByUsernameAsync(username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _repository.GetByEmailAsync(email);
        }

        public async Task<bool> UsernameExistsAsync(string username, Guid? id = null)
        {
            return await _repository.UsernameExistsAsync(username, id);
        }

        public async Task<bool> EmailExistsAsync(string email, Guid? id = null)
        {
            return await _repository.EmailExistsAsync(email, id);
        }

        public async Task<bool> IsAdminAsync(Guid idUsuario)
        {
            return await _repository.IsAdminAsync(idUsuario);
        }

        #endregion
    }

    #region Validator

    public partial class UserDomain
    {
        public async Task ValidateCreationAsync(User user)
        {
            var usernameExists = await _repository.UsernameExistsAsync(user.Username, user.Id);
            if (usernameExists)
                UserException.UsernameAlreadyRegistered();

            var emailExists = await _repository.EmailExistsAsync(user.Email, user.Id);
            if (emailExists)
                UserException.EmailAlreadyRegistered();
        }

        public async Task ValidateChangeAsync(User user)
        {
            await ValidateCreationAsync(user);
            ValidateIfUserExists(user);
        }

        public async Task ValidateCreationByDomainAsync(User user, string domainName)
        {
            var usernameExists = await _repository.UsernameExistsAsync(user.Username, domainName);
            if (usernameExists)
                UserException.UsernameAlreadyRegistered();

            var emailExists = await _repository.EmailExistsAsync(user.Email, domainName);
            if (emailExists)
                UserException.EmailAlreadyRegistered();
        }

        public static void ValidateRemoval(User? user)
        {
            ValidateIfUserExists(user);
        }

        private static void ValidateIfUserExists(User? user)
        {
            if (user is null)
                UserException.UserNotFound();
        }
    }

    #endregion
}
