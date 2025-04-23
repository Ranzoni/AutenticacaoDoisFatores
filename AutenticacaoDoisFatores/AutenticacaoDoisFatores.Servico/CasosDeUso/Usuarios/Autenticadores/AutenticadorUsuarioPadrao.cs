using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores
{
    public class AutenticadorUsuarioPadrao(DominioDeUsuarios dominio, DominioDePermissoes permissoes, INotificador notificador) : ITipoDeAutenticador
    {
        private readonly DominioDeUsuarios _dominio = dominio;
        private readonly DominioDePermissoes _permissoes = permissoes;
        private readonly INotificador _notificador = notificador;

        public async Task<object?> ExecutarAsync(Usuario usuario)
        {
            if (!AutenticacaoEhValida(usuario))
                return null;

            var permissoesUsuario = await RetornarPermissoesAsync(usuario);
            var token = Seguranca.GerarTokenAutenticacaoUsuario(usuario.Id, permissoesUsuario);

            usuario.AtualizarDataUltimoAcesso();
            await _dominio.AlterarAsync(usuario);

            var usuarioCadastrado = (UsuarioCadastrado)usuario;
            return new UsuarioAutenticado(usuario: usuarioCadastrado, token: token);
        }

        private bool AutenticacaoEhValida(Usuario usuario)
        {
            if (!usuario.Ativo)
            {
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);
                return false;
            }

            return true;
        }

        private async Task<IEnumerable<TipoDePermissao>> RetornarPermissoesAsync(Usuario usuario)
        {
            if (usuario.EhAdmin)
                return DominioDePermissoes.RetornarTodasPermissoes();
                
            return await _permissoes.RetornarPermissoesAsync(usuario.Id);
        }
    }
}
