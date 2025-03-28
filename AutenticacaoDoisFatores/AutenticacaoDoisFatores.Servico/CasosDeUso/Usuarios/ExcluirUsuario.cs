using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios
{
    public class ExcluirUsuario(DominioDeUsuarios dominio, INotificador notificador)
    {
        private readonly DominioDeUsuarios _dominio = dominio;
        private readonly INotificador _notificador = notificador;

        public async Task ExecutarAsync(Guid id)
        {
            if (!await ExclusaoEhValidaAsync(id))
                return;

            await _dominio.ExcluirUsuarioAsync(id);
        }

        private async Task<bool> ExclusaoEhValidaAsync(Guid id)
        {
            var usuario = await _dominio.BuscarUnicoAsync(id);
            if (usuario is null || usuario.EhAdmin)
            {
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);
                return false;
            }

            return true;
        }
    }
}
