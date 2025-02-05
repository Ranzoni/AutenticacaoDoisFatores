using AutenticacaoDoisFatores.Dominio.Compartilhados.Repositorios;
using AutenticacaoDoisFatores.Dominio.Entidades;

namespace AutenticacaoDoisFatores.Dominio.Repositorios
{
    public interface IRepositorioDeUsuarios : IRepositorioBase<Usuario>
    {
        Task<bool> ExisteNomeUsuarioAsync(string nomeUsuario);
        Task<bool> ExisteEmailAsync(string email);
    }
}
