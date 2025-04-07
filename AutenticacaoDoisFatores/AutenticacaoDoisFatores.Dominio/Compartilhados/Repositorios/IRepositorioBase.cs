using AutenticacaoDoisFatores.Dominio.Filtros;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Repositorios
{
    public interface IRepositorioBase<T, TFiltro> where TFiltro : FiltroPadrao
    {
        void Adicionar(T entidade);
        void Editar(T entidade);
        void Excluir(T entidade);
        Task<T?> BuscarUnicoAsync(Guid id);
        Task<IEnumerable<T>> BuscarVariosAsync(TFiltro filtros);
        Task SalvarAlteracoesAsync();
    }
}
