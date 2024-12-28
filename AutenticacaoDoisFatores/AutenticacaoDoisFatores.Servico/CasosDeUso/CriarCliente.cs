using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Validadores;
using AutenticacaoDoisFatores.Servico.DTO;
using AutoMapper;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso
{
    public class CriarCliente(IMapper mapper, DominioDeClientes dominio)
    {
        private readonly IMapper _mapper = mapper;
        private readonly DominioDeClientes _dominio = dominio;

        public async Task<ClienteCadastrado?> ExecutarAsync(NovoCliente novoCliente)
        {
            if (!NovoClienteEhValido(novoCliente))
                return null;

            var cliente = _mapper.Map<Cliente>(novoCliente);

            await _dominio.CriarAsync(cliente);

            var clienteCadastrado = _mapper.Map<ClienteCadastrado>(cliente);
            return clienteCadastrado;
        }

        public static bool NovoClienteEhValido(NovoCliente novoCliente)
        {
            return ValidadorDeCliente.NomeEhValido(novoCliente.Nome) && ValidadorDeCliente.EmailEhValido(novoCliente.Email);
        }
    }
}
