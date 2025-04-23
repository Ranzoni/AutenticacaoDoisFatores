using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios
{
    public class AutenticarUsuarioEmDoisFatores(DominioDeCodDeAutenticacao dominio, DominioDeUsuarios usuarios, RetornarUsuarioAutenticado autenticador, DominioAppAutenticador appAutenticador, INotificador notificador)
    {
        private readonly DominioDeCodDeAutenticacao _dominio = dominio;
        private readonly DominioDeUsuarios _usuarios = usuarios;
        private readonly RetornarUsuarioAutenticado _autenticador = autenticador;
        private readonly DominioAppAutenticador _appAutenticador = appAutenticador;
        private readonly INotificador _notificador = notificador;

        public async Task<UsuarioAutenticado?> ExecutarAsync(Guid idUsuario, string codigo)
        {
            var usuario = await _usuarios.BuscarUnicoAsync(idUsuario);
            if (!UsuarioEhValido(usuario))
                return null;

            if (!await CodigoEhValidoAsync(usuario!, codigo))
                return null;

            var usuarioAutenticado = await _autenticador.ExecutarAsync(usuario!);
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
                return ValidarCodigoPorApp(codigo);
            else
                return await ValidarCodigoPorCriptografiaAsync(usuario.Id, codigo);
        }

        private bool ValidarCodigoPorApp(string codigo)
        {
            if (!_appAutenticador.CodigoEhValido(codigo))
            {
                _notificador.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);
                return false;
            }

            return true;
        }

        private async Task<bool> ValidarCodigoPorCriptografiaAsync(Guid idUsuario, string codigo)
        {
            var codigoSalvo = await _dominio.BuscarCodigoAsync(idUsuario);
            var codigoDigitadoCriptografado = Criptografia.CriptografarComSha512(codigo);
            if (!codigoDigitadoCriptografado.Equals(codigoSalvo))
            {
                _notificador.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);
                return false;
            }

            return true;
        }
    }
}
