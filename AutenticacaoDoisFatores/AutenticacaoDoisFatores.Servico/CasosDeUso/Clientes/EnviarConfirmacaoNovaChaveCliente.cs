using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes
{
    public class EnviarConfirmacaoNovaChaveCliente(DominioDeClientes dominio, INotificador notificador, EnvioDeEmail email)
    {
        private readonly DominioDeClientes _dominio = dominio;
        private readonly INotificador _notificador = notificador;
        private readonly EnvioDeEmail _email = email;

        public async Task EnviarAsync(string email, string linkBaseConfirmacaoCadastro)
        {
            var cliente = await _dominio.BuscarPorEmailAsync(email);

            if (!EnvioEhValido(cliente) || cliente is null)
                return;

            var tokenGeracaoNovaChave = Seguranca.GerarTokenDeGeracaoNovaChaveDeAcesso(cliente.Id);
            _email.EnviarConfirmacaoDeNovaChaveCliente(cliente.Email, linkBaseConfirmacaoCadastro, tokenGeracaoNovaChave);
        }

        private bool EnvioEhValido(Cliente? cliente)
        {
            if (cliente is null)
            {
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoCliente.ClienteNaoEncontrado);
                return false;
            }

            var ehValido = !_notificador.ExisteMensagem();
            return ehValido;
        }
    }
}
