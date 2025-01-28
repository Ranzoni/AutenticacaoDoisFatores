using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Clientes
{
    public class ReenviarChaveCliente(DominioDeClientes dominio, INotificador notificador, EnvioDeEmail email)
    {
        private readonly DominioDeClientes _dominio = dominio;
        private readonly INotificador _notificador = notificador;
        private readonly EnvioDeEmail _email = email;

        public async Task ReenviarAsync(string email, string linkBaseConfirmacaoCadastro)
        {
            var cliente = await _dominio.BuscarPorEmailAsync(email);

            if (!ReenvioEhValido(cliente) || cliente is null)
                return;

            var (chave, chaveCriptografada) = Seguranca.GerarChaveDeAcessoComCriptografia();

            await AlterarChaveAcessoAsync(cliente, chaveCriptografada);
            EnviarEmail(cliente.Id, cliente.Email, chave, linkBaseConfirmacaoCadastro);
        }

        private async Task AlterarChaveAcessoAsync(Cliente cliente, string chave)
        {
            cliente.AlterarChaveAcesso(chave);
            await _dominio.AlterarClienteAsync(cliente);
        }

        private void EnviarEmail(Guid id, string email, string chave, string linkBaseConfirmacaoCadastro)
        {
            var tokenConfirmacaoDeCadastro = Seguranca.GerarTokenDeConfirmacaoDeCliente(id);
            _email.EnviarConfirmacaoDeCadastroDeCliente(email, chave, linkBaseConfirmacaoCadastro, tokenConfirmacaoDeCadastro);
        }

        private bool ReenvioEhValido(Cliente? cliente)
        {
            if (cliente is null)
            {
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoCliente.ClienteNaoEncontrado);
                return false;
            }

            if (cliente.Ativo)
                _notificador.AddMensagem(MensagensValidacaoCliente.ClienteJaAtivado);

            var ehValido = !_notificador.ExisteMensagem();
            return ehValido;
        }
    }
}
