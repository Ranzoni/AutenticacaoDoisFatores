using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores.AutenticacoesDoisFatores
{
    public class AutenticadorUsuarioEmDoisFatoresPorApp(DominioAppAutenticador autenticadorPorApp, INotificador notificador) : ITipoDeAutentidorUsuarioEmDoisFatores
    {
        private readonly DominioAppAutenticador _appAutenticador = autenticadorPorApp;
        private readonly INotificador _notificador = notificador;

        public Task<RespostaAutenticacaoDoisFatores?> EnviarAsync(Usuario usuario)
        {
            if (!EnvioEhValido(usuario))
                return Task.FromResult<RespostaAutenticacaoDoisFatores?>(null);

            var qrCode = _appAutenticador.GerarQrCode(usuario);

            var token = Seguranca.GerarTokenCodAutenticacaoUsuario(usuario.Id);
            var resposta = new RespostaAutenticacaoDoisFatores(token, qrCode);
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
