using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes
{
    public class AtivarCliente(DominioDeClientes dominio, INotificador notificador)
    {
        private readonly DominioDeClientes _dominio = dominio;
        private readonly INotificador _notificador = notificador;

        public async Task AtivarAsync(string token)
        {
            var idCliente = Seguranca.RetornarIdClienteTokenDeConfirmacaoDeCliente(token);

            var ativacaoEhValida = AtivacaoDeClienteEhValida(idCliente);
            if (!ativacaoEhValida)
                return;

            var cliente = await _dominio.BuscarClienteAsync(idCliente);
            if (!AtivacaoDeClienteEhValida(cliente) || cliente is null)
                return;

            cliente.Ativar(true);
            await _dominio.AlterarClienteAsync(cliente);
        }

        private bool AtivacaoDeClienteEhValida(Guid idCliente)
        {
            if (idCliente.Equals(Guid.Empty))
                _notificador.AddMensagem(MensagensValidacaoCliente.TokenInvalido);

            var estaValido = !_notificador.ExisteMensagem();
            return estaValido;
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
