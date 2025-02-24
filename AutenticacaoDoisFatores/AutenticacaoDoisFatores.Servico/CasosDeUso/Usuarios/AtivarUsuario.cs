using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios
{
    public class AtivarUsuario(DominioDeUsuarios dominio, INotificador notificador)
    {
        private readonly DominioDeUsuarios _dominio = dominio;
        private readonly INotificador _notificador = notificador;

        public async Task AtivarAsync(Guid idUsuario)
        {
            var usuario = await _dominio.BuscarUnicoAsync(idUsuario);
            if (!AtivacaoEhValida(usuario) || usuario is null)
                return;

            usuario.Ativar(true);
            await _dominio.AlterarUsuarioAsync(usuario);
        }

        public bool AtivacaoEhValida(Usuario? usuario)
        {
            if (usuario is null)
            {
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);
                return false;
            }

            if (usuario.Ativo)
                _notificador.AddMensagem(MensagensValidacaoUsuario.UsuarioJaAtivado);

            return !_notificador.ExisteMensagem();
        }
    }
}
