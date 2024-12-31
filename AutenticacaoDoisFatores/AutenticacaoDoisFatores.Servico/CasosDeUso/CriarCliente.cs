using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Validadores;
using AutenticacaoDoisFatores.Servico.DTO;
using AutoMapper;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso
{
    public class CriarCliente(IMapper mapper, DominioDeClientes dominio, INotificador notificador)
    {
        private readonly IMapper _mapper = mapper;
        private readonly DominioDeClientes _dominio = dominio;
        private readonly INotificador _notificador = notificador;

        public async Task<ClienteCadastrado?> ExecutarAsync(NovoCliente novoCliente)
        {
            if (!NovoClienteEhValido(novoCliente))
                return null;

            var cliente = _mapper.Map<Cliente>(novoCliente);

            await _dominio.CriarClienteAsync(cliente);
            await _dominio.CriarDominioDoClienteAsync(cliente.Id);

            var clienteCadastrado = _mapper.Map<ClienteCadastrado>(cliente);
            return clienteCadastrado;
        }

        private bool NovoClienteEhValido(NovoCliente novoCliente)
        {
            if (!ValidadorDeCliente.NomeEhValido(novoCliente.Nome))
            {
                _notificador.AddMensagem(MensagensCliente.NomeInvalido);
                return false;
            }

            if (!ValidadorDeCliente.EmailEhValido(novoCliente.Email))
            {
                _notificador.AddMensagem(MensagensCliente.EmailInvalido);
                return false;
            }

            return true;
        }
    }
}
