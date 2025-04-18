namespace AutenticacaoDoisFatores.Dominio.Repositorios
{
    public interface IRepositorioDeCodigoDeAutenticacao
    {
        public Task SalvarAsync(Guid idUsuario, string codigo);
        public Task<string?> BuscarCodigoAsync(Guid idUsuario);
    }
}
