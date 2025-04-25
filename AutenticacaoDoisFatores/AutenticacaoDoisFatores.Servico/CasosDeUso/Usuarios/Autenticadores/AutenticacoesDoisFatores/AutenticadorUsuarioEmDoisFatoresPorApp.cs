using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores.AutenticacoesDoisFatores
{
    public class AutenticadorUsuarioEmDoisFatoresPorApp(INotificador notificador) : ITipoDeAutentidorUsuarioEmDoisFatores
    {
        private readonly INotificador _notificador = notificador;

        public Task<RespostaAutenticacaoDoisFatores?> EnviarAsync(Usuario usuario)
        {
            if (!EnvioEhValido(usuario))
                return Task.FromResult<RespostaAutenticacaoDoisFatores?>(null);

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
