using AutenticacaoDoisFatores.Dominio.Compartilhados.Repositorios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Filtros;

namespace AutenticacaoDoisFatores.Dominio.Repositorios
{
    public interface IRepositorioDeUsuarios : IRepositorioBase<Usuario, FiltroDeUsuarios>
    {
        Task<bool> ExisteNomeUsuarioAsync(string nomeUsuario, Guid? id = null);
        Task<bool> ExisteEmailAsync(string email, Guid? id = null);
        Task<Usuario?> BuscarPorNomeUsuarioAsync(string nomeUsuario);
        Task<Usuario?> BuscarPorEmailAsync(string email);
        void Adicionar(Usuario entidade, string dominio);
        Task<bool> ExisteNomeUsuarioAsync(string nomeUsuario, string dominio);
        Task<bool> ExisteEmailAsync(string email, string dominio);
        Task<bool> EhAdmAsync(Guid id);
        Task<Usuario?> BuscarUsuarioPorDominioAsync(Guid id, string dominio);
    }
}
