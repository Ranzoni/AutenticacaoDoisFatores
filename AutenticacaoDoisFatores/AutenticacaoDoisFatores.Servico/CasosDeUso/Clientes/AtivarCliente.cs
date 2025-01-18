using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes
{
    public class AtivarCliente(DominioDeClientes dominio, INotificador notificador)
    {
        private readonly DominioDeClientes _dominio = dominio;
        private readonly INotificador _notificador = notificador;

        public async Task AtivarAsync(Guid idCliente)
        {
            var cliente = await _dominio.BuscarClienteAsync(idCliente);
            if (!AtivacaoDeClienteEhValida(cliente) || cliente is null)
                return;

            cliente.Ativar(true);
            await _dominio.AlterarClienteAsync(cliente);
        }

        private bool AtivacaoDeClienteEhValida(Cliente? cliente)
        {
            if (cliente is null)
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoCliente.ClienteNaoEncontrado);

            if (cliente?.Ativo ?? false)
                _notificador.AddMensagem(MensagensValidacaoCliente.ClienteJaAtivado);

            var estaValido = !_notificador.ExisteMensagem();
            return estaValido;
        }
    }
}
