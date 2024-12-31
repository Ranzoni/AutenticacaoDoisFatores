namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Repositorios
{
    public interface IReporioBase<T>
    {
        void Adicionar(T entidade);
        void Editar(T entidade);
        void Excluir(Guid id);
        Task<T?> BuscarUnicoAsync(Guid id);
        Task SalvarAlteracoesAsync();
    }
}
