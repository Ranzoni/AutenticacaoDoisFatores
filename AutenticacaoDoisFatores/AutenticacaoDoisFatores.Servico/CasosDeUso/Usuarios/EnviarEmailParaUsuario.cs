using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios
{
    public class EnviarEmailParaUsuario(EnvioDeEmail email, DominioDeUsuarios usuarios, INotificador notificador)
    {
        private readonly EnvioDeEmail _email = email;
        private readonly DominioDeUsuarios _usuarios = usuarios;
        private readonly INotificador _notificador = notificador;

        public async Task ExecutarAsync(Guid idUsuario, EnvioEmailParaUsuario envioEmailParaUsuario)
        {
            var usuario = await _usuarios.BuscarUnicoAsync(idUsuario);
            if (!EnvioEhValido(usuario))
                return;

            _email.EnviarPersonalizadoParaUsuario(usuario!.Email, envioEmailParaUsuario.Titulo, envioEmailParaUsuario.Mensagem, envioEmailParaUsuario.HtmlEmail);
        }

        private bool EnvioEhValido(Usuario? usuario)
        {
            if (usuario is null || usuario.EhAdmin)
            {
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);
                return false;
            }

            return true;
        }
    }
}
