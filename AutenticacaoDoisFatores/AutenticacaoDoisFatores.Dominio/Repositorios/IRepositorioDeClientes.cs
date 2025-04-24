using AutenticacaoDoisFatores.Dominio.Compartilhados.Repositorios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Filtros;

namespace AutenticacaoDoisFatores.Dominio.Repositorios
{
    public interface IRepositorioDeClientes : IRepositorioBase<Cliente, FiltroDeClientes>
    {
        Task CriarDominio(string nomeDominio);
        Task<bool> ExisteDominioAsync(string nomeDominio);
        Task<bool> ExisteEmailAsync(string email);
        Task<Cliente?> BuscarPorEmailAsync(string email);
        Task<string?> RetornarNomeDominioAsync(string chave);
    }
}
