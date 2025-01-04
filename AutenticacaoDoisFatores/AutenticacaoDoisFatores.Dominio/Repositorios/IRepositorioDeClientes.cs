using AutenticacaoDoisFatores.Dominio.Compartilhados.Repositorios;
using AutenticacaoDoisFatores.Dominio.Entidades;

namespace AutenticacaoDoisFatores.Dominio.Repositorios
{
    public interface IRepositorioDeClientes : IReporioBase<Cliente>
    {
        Task CriarDominio(string nomeDominio);
        Task<bool> ExisteDominio(string nomeDominio);
        Task<bool> ExisteEmail(string email);
    }
}
