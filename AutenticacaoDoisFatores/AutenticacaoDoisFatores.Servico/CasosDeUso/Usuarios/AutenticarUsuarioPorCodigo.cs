using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios
{
    public class AutenticarUsuarioPorCodigo(GerenciadorDeCodAutenticacao gerenciadorCodAutenticacao, DominioDeUsuarios usuarios, AutenticadorUsuarioPadrao autenticadorPadrao, AppAutenticador autenticadorPorApp, INotificador notificador)
    {
        private readonly GerenciadorDeCodAutenticacao _gerenciadorCodAutenticacao = gerenciadorCodAutenticacao;
        private readonly DominioDeUsuarios _usuarios = usuarios;
        private readonly AutenticadorUsuarioPadrao _autenticadorPadrao = autenticadorPadrao;
        private readonly AppAutenticador _autenticadorPorApp = autenticadorPorApp;
        private readonly INotificador _notificador = notificador;

        public async Task<UsuarioAutenticado?> ExecutarAsync(Guid idUsuario, string codigo)
        {
            var usuario = await _usuarios.BuscarUnicoAsync(idUsuario);
            if (!UsuarioEhValido(usuario))
                return null;

            if (!await CodigoEhValidoAsync(usuario!, codigo))
                return null;

            var usuarioAutenticado = await _autenticadorPadrao.ExecutarAsync(usuario!);
            return (UsuarioAutenticado?)usuarioAutenticado;
        }

        private bool UsuarioEhValido(Usuario? usuario)
        {
            if (usuario is null || !usuario.Ativo)
            {
                _notificador.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);
                return false;
            }

            return true;
        }

        private async Task<bool> CodigoEhValidoAsync(Usuario usuario, string codigo)
        {
            if (usuario.TipoDeAutenticacao.Equals(TipoDeAutenticacao.AppAutenticador))
                return ValidarCodigoPorApp(codigo, usuario);
            else
                return await ValidarCodigoPorCriptografiaAsync(usuario.Id, codigo);
        }

        private bool ValidarCodigoPorApp(string codigo, Usuario usuario)
        {
            if (!_autenticadorPorApp.CodigoEhValido(codigo, usuario))
            {
                _notificador.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.NaoAutenticado);
                return false;
            }

            return true;
        }

        private async Task<bool> ValidarCodigoPorCriptografiaAsync(Guid idUsuario, string codigo)
        {
            var codigoSalvo = await _gerenciadorCodAutenticacao.BuscarCodigoAsync(idUsuario);
            var codigoDigitadoCriptografado = Criptografia.CriptografarComSha512(codigo);
            if (!codigoDigitadoCriptografado.Equals(codigoSalvo))
            {
                _notificador.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.NaoAutenticado);
                return false;
            }

            return true;
        }
    }
}
