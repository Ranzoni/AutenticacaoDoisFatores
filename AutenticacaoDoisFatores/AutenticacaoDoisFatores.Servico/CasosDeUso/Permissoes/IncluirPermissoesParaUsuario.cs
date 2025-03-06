using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Permissoes
{
    public class IncluirPermissoesParaUsuario(DominioDePermissoes dominio, DominioDeUsuarios usuarios, INotificador notificador)
    {
        private readonly DominioDePermissoes _dominio = dominio;
        private readonly DominioDeUsuarios _usuarios = usuarios;
        private readonly INotificador _notificador = notificador;

        public async Task ExecutarAsync(Guid idUsuario, IEnumerable<TipoDePermissao> permissoes)
        {
            var usuario = await _usuarios.BuscarUnicoAsync(idUsuario);
            if (!InclusaoPermissoesEhValida(usuario))
                return;

            await _dominio.AdicionarAsync(idUsuario, permissoes);
        }

        private bool InclusaoPermissoesEhValida(Usuario? usuario)
        {
            if (usuario is null || !usuario.Ativo || usuario.EhAdmin)
            {
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);
                return false;
            }

            return true;
        }
    }
}
