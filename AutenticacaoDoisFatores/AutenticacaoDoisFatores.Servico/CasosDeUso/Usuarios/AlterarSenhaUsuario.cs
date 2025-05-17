using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios
{
    public class AlterarSenhaUsuario(DominioDeUsuarios dominio, INotificador notificador)
    {
        private readonly DominioDeUsuarios _dominio = dominio;
        private readonly INotificador _notificador = notificador;

        public async Task ExecutarAsync(Guid idUsuario, string novaSenha)
        {
            if (!SenhaEhValida(novaSenha))
                return;

            var usuario = await _dominio.BuscarUnicoAsync(idUsuario);
            if (!AlteracaoSenhaEhValida(usuario) || usuario is null)
                return;

            var novaSenhaCriptografada = Criptografia.CriptografarComSha512(novaSenha);
            usuario.AlterarSenha(novaSenhaCriptografada);
            await _dominio.AlterarAsync(usuario);
        }

        private bool SenhaEhValida(string novaSenha)
        {
            if (!Seguranca.ComposicaoSenhaEhValida(novaSenha))
            {
                _notificador.AddMensagem(MensagensValidacaoUsuario.SenhaInvalida);
                return false;
            }

            return true;
        }

        private bool AlteracaoSenhaEhValida(Usuario? usuario)
        {
            if (usuario is null || !usuario.Ativo || usuario.EhAdmin)
            {
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);
                return false;
            }

            return !_notificador.ExisteMensagem();
        }
    }
}
