using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios
{
    public class EnviarEmailAtivacao(EnvioDeEmail email, DominioDeUsuarios usuarios, INotificador notificador)
    {
        private readonly EnvioDeEmail _email = email;
        private readonly DominioDeUsuarios _usuarios = usuarios;
        private readonly INotificador _notificador = notificador;

        public async Task ExecutarAsync(Guid idUsuario, string linkAtivacao)
        {
            var usuario = await _usuarios.BuscarUnicoAsync(idUsuario);
            if (!EnvioEhValido(usuario))
                return;

            _email.EnviarAtivacaoDeUsuario(usuario!.Email, linkAtivacao);
        }

        private bool EnvioEhValido(Usuario? usuario)
        {
            if (usuario is null || usuario.EhAdmin)
            {
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);
                return false;
            }

            if (usuario.Ativo)
            {
                _notificador.AddMensagem(MensagensValidacaoUsuario.UsuarioJaAtivo);
                return false;
            }

            return true;
        }
    }
}
