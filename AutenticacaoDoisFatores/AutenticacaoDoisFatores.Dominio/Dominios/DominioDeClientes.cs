using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public class DominioDeClientes(IRepositorioDeClientes repositorio)
    {
        private readonly IRepositorioDeClientes _repositorio = repositorio;

        public async Task<Cliente> CriarAsync(Cliente cliente)
        {
            _repositorio.Adicionar(cliente);
            await _repositorio.SalvarAlteracoesAsync();

            return cliente;
        }

        public async Task CriarDominioDoClienteAsync(Guid idCliente)
        {
            var cliente = await _repositorio.BuscarUnicoAsync(idCliente);
            await _repositorio.CriarDominio(cliente.NomeDominio);
        }
    }
}
