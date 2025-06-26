using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Infra.Utils.Migrants;
using AutenticacaoDoisFatores.Infra.Contexts;
using Microsoft.EntityFrameworkCore;
using AutenticacaoDoisFatores.Domain.Filters;
using AutenticacaoDoisFatores.Domain.Shared;

namespace AutenticacaoDoisFatores.Infra.Repositories
{
    public class ClientRepository(BaseContext context, IMigration migration) : IClientRepository
    {
        private readonly BaseContext _context = context;

        #region Write

        public void Add(Client entity)
        {
            _context.Add(entity);
        }

        public async Task NewDomainAsync(string domainName)
        {
            var sql = $"CREATE SCHEMA IF NOT EXISTS {domainName};";

            await _context.Database.ExecuteSqlRawAsync(sql);
            await migration.ApplyMigrationsAsync(domainName);
        }

        public void Update(Client entity)
        {
            _context.Update(entity);
        }

        public void Remove(Client entity)
        {
            throw new NotImplementedException();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Read

        public Task<Client?> GetByIdAsync(Guid id)
        {
            return _context.Clients.FirstOrDefaultAsync(c => c.Id.Equals(id));
        }

        public async Task<bool> DomainExistsAsync(string domainName)
        {
            return await _context.Clients.AnyAsync(c => c.DomainName.ToLower().Trim().Equals(domainName.ToLower().Trim()));
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Clients.AnyAsync(c => c.Email.ToLower().Trim().Equals(email.ToLower().Trim()));
        }

        public async Task<Client?> GetByEmailAsync(string email)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Email.Equals(email));
        }

        public async Task<string?> GetDomainNameByAccessKeyAsync(string accessKey)
        {
            var cliente = await _context.Clients.FirstOrDefaultAsync(c => c.Active && c.AccessKey.Equals(accessKey));
            return cliente?.DomainName;
        }

        public async Task<IEnumerable<Client>> GetAllAsync(ClientFilter filter)
        {
            var qtToSkip = (filter.Page - 1) * filter.Quantity;

            IQueryable<Client> query = _context.Clients;

            if (!filter.Name!.IsNullOrEmptyOrWhiteSpaces())
                query = query.Where(c => c.Name.ToLower().Contains(filter.Name!.ToLower()));

            if (!filter.Email!.IsNullOrEmptyOrWhiteSpaces())
                query = query.Where(c => c.Email.ToLower().Contains(filter.Email!.ToLower()));

            if (!filter.DomainName!.IsNullOrEmptyOrWhiteSpaces())
                query = query.Where(c => c.DomainName.ToLower().Contains(filter.DomainName!.ToLower()));

            if (filter.Active.HasValue)
                query = query.Where(c => c.Active.Equals(filter.Active));

            if (filter.CreatedFrom.HasValue)
                query = query.Where(c => c.CreatedAt.Date >= filter.CreatedFrom.Value.Date);

            if (filter.CreatedUntil.HasValue)
                query = query.Where(c => c.CreatedAt.Date <= filter.CreatedUntil.Value.Date);

            if (filter.UpdatedFrom.HasValue)
                query = query.Where(c => c.UpdatedAt!.Value.Date >= filter.UpdatedFrom.Value.Date);

            if (filter.UpdatedUntil.HasValue)
                query = query.Where(c => c.UpdatedAt!.Value.Date <= filter.UpdatedUntil.Value.Date);

            return await query
                .Skip(qtToSkip)
                .Take(filter.Quantity)
                .ToListAsync();
        }

        #endregion
    }
}
