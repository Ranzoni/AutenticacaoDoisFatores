using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;

namespace AutenticacaoDoisFatores.Dominio.Repositorios
{
    public interface IRepositorioDePermissoes
    {
        Task AdicionarAsync(Guid idUsuario, IEnumerable<TipoDePermissao> permissoes);
        Task<IEnumerable<TipoDePermissao>> RetornarPorUsuarioAsync(Guid idUsuario);
        Task EditarAsync(Guid idUsuario, IEnumerable<TipoDePermissao> permissoes);
    }
}
