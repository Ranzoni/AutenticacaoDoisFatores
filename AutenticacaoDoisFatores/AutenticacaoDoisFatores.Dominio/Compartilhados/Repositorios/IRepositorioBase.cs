namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Repositorios
{
    public interface IRepositorioBase<T>
    {
        void Adicionar(T entidade);
        void Editar(T entidade);
        void Excluir(Guid id);
        Task<T?> BuscarUnicoAsync(Guid id);
        Task SalvarAlteracoesAsync();
    }
}
