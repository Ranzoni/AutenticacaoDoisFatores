using AutenticacaoDoisFatores.Domain.Filters;

namespace AutenticacaoDoisFatores.Domain.Shared.Repositories
{
    public interface IBaseRepository<T, TFilter> where TFilter : BaseFilter
    {
        void Add(T entidade);
        void Update(T entidade);
        void Remove(T entidade);
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync(TFilter filtros);
        Task SaveChangesAsync();
    }
}
