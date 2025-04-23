using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores.AutenticacoesDoisFatores
{
    public class AutenticadorUsuarioEmDoisFatoresPorApp(DominioAppAutenticador autenticadorPorApp, EnvioDeEmail email, INotificador notificador, string linkBaseParaQrCode) : ITipoDeAutentidorUsuarioEmDoisFatores
    {
        private readonly DominioAppAutenticador _appAutenticador = autenticadorPorApp;
        private readonly EnvioDeEmail _email = email;
        private readonly INotificador _notificador = notificador;

        public Task<RespostaAutenticacaoDoisFatores?> EnviarAsync(Usuario usuario)
        {
            if (!EnvioEhValido(usuario))
                return Task.FromResult<RespostaAutenticacaoDoisFatores?>(null);

            var qrCode = _appAutenticador.GerarQrCode(usuario);
            if (qrCode is null || qrCode.EstaVazio())
                ExcecoesAppAutenticador.QrCodeNaoGerado();

            _email.EnviarQrCodeAutenticacaoDoisFatores(usuario.Email, linkBaseParaQrCode, qrCode!);

            var token = Seguranca.GerarTokenCodAutenticacaoUsuario(usuario.Id);
            var resposta = new RespostaAutenticacaoDoisFatores(token);
            return Task.FromResult<RespostaAutenticacaoDoisFatores?>(resposta);
        }

        private bool EnvioEhValido(Usuario usuario)
        {
            if (!usuario.Ativo)
            {
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);
                return false;
            }

            return true;
        }
    }
}
