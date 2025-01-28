using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes
{
    public class GerarNovaChaveAcessoCliente(DominioDeClientes dominio, INotificador notificador, EnvioDeEmail email)
    {
        private readonly DominioDeClientes _dominio = dominio;
        private readonly INotificador _notificador = notificador;
        private readonly EnvioDeEmail _email = email;

        public async Task GerarNovaChaveAsync(Guid idCliente)
        {
            var cliente = await _dominio.BuscarClienteAsync(idCliente);

            if (!GeracaoEhValida(cliente) || cliente is null)
                return;

            var (chave, chaveCriptografada) = Seguranca.GerarChaveDeAcessoComCriptografia();

            cliente.AlterarChaveAcesso(chaveCriptografada);
            await _dominio.AlterarClienteAsync(cliente);

            _email.EnviarConfirmacaoDeNovaChaveCliente(cliente.Email, chave);
        }

        private bool GeracaoEhValida(Cliente? cliente)
        {
            if (cliente is null)
            {
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoCliente.ClienteNaoEncontrado);
                return false;
            }

            if (!cliente.Ativo)
                _notificador.AddMensagem(MensagensValidacaoCliente.ClienteNaoAtivo);

            var ehValido = !_notificador.ExisteMensagem();
            return ehValido;
        }
    }
}
