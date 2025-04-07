using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Filtros;
using AutenticacaoDoisFatores.Servico.DTO.Clientes;
using AutoMapper;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes
{
    public class BuscarClientes(DominioDeClientes dominio, IMapper mapeador)
    {
        private readonly DominioDeClientes _dominio = dominio;
        private readonly IMapper _mapeador = mapeador;

        public async Task<ClienteCadastrado?> BuscarUnicoAsync(Guid id)
        {
            var cliente = await _dominio.BuscarClienteAsync(id);

            var clienteCadastrado = _mapeador.Map<ClienteCadastrado?>(cliente);
            return clienteCadastrado;
        }

        public async Task<IEnumerable<ClienteCadastrado>> BuscarVariosAsync(FiltrosParaBuscarClientes filtrosParaBuscar)
        {
            var filtros = _mapeador.Map<FiltroDeClientes>(filtrosParaBuscar);

            var clientes = await _dominio.BuscarVariosAsync(filtros);

            var clientesCadastrados = _mapeador.Map<IEnumerable<ClienteCadastrado>>(clientes);
            return clientesCadastrados;
        }
    }
}
