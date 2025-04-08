using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Filtros;
using AutenticacaoDoisFatores.Servico.DTO.Clientes;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes
{
    public class BuscarClientes(DominioDeClientes dominio)
    {
        private readonly DominioDeClientes _dominio = dominio;

        public async Task<ClienteCadastrado?> BuscarUnicoAsync(Guid id)
        {
            var cliente = await _dominio.BuscarClienteAsync(id);
            return (ClienteCadastrado?)cliente!;
        }

        public async Task<IEnumerable<ClienteCadastrado>> BuscarVariosAsync(FiltrosParaBuscarClientes filtrosParaBuscar)
        {
            var filtros = (FiltroDeClientes)filtrosParaBuscar;

            var clientes = await _dominio.BuscarVariosAsync(filtros);

            var clientesCadastrados = clientes.Select(c => (ClienteCadastrado)c);
            return clientesCadastrados;
        }
    }
}
