using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Validadores;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios
{
    public class AlterarUsuario(DominioDeUsuarios dominio, INotificador notificador)
    {
        private readonly DominioDeUsuarios _dominio = dominio;
        private readonly INotificador _notificador = notificador;

        public async Task<UsuarioCadastrado?> ExecutarAsync(Guid id, NovosDadosUsuario novosDadosUsuario)
        {
            var usuarioParaAlterar = await _dominio.BuscarUnicoAsync(id);
            if (!await AlteracaoEhValida(id, usuarioParaAlterar, novosDadosUsuario) || usuarioParaAlterar is null)
                return null;

            if (!usuarioParaAlterar.Nome.Equals(novosDadosUsuario.Nome))
                usuarioParaAlterar.AlterarNome(novosDadosUsuario.Nome);

            if (!usuarioParaAlterar.NomeUsuario.Equals(novosDadosUsuario.NomeUsuario))
                usuarioParaAlterar.AlterarNomeUsuario(novosDadosUsuario.NomeUsuario);

            if (!usuarioParaAlterar.Celular.Equals(novosDadosUsuario.Celular))
                usuarioParaAlterar.AlterarCelular(novosDadosUsuario.Celular);

            if (!usuarioParaAlterar.TipoDeAutenticacao.Equals(novosDadosUsuario.TipoDeAutenticacao))
                usuarioParaAlterar.ConfigurarTipoDeAutenticacao(novosDadosUsuario.TipoDeAutenticacao);

            await _dominio.AlterarAsync(usuarioParaAlterar);

            return (UsuarioCadastrado)usuarioParaAlterar;
        }

        private async Task<bool> AlteracaoEhValida(Guid id, Usuario? usuarioParaAlterar, NovosDadosUsuario novosDadosUsuario)
        {
            Task<bool>? verificarNomeUsuarioJaCadastrado = null;
            Task<bool>? verificarEmailJaCadastrado = null;

            if (usuarioParaAlterar is null || !usuarioParaAlterar.Ativo || usuarioParaAlterar.EhAdmin)
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);

            if (!ValidadorDeUsuario.NomeEhValido(novosDadosUsuario.Nome))
                _notificador.AddMensagem(MensagensValidacaoUsuario.NomeInvalido);

            if (!ValidadorDeUsuario.NomeUsuarioEhValido(novosDadosUsuario.NomeUsuario))
                _notificador.AddMensagem(MensagensValidacaoUsuario.NomeUsuarioInvalido);
            else 
                verificarNomeUsuarioJaCadastrado = _dominio.ExisteNomeUsuarioAsync(novosDadosUsuario.NomeUsuario, id);

            if (!ValidadorDeUsuario.CelularEhValido(novosDadosUsuario.Celular))
                _notificador.AddMensagem(MensagensValidacaoUsuario.CelularInvalido);

            if (verificarNomeUsuarioJaCadastrado is not null)
            {
                var existeNomeUsuario = await verificarNomeUsuarioJaCadastrado;
                if (existeNomeUsuario)
                    _notificador.AddMensagem(MensagensValidacaoUsuario.NomeUsuarioJaCadastrado);
            }

            if (verificarEmailJaCadastrado is not null)
            {
                var existeEmail = await verificarEmailJaCadastrado;
                if (existeEmail)
                    _notificador.AddMensagem(MensagensValidacaoUsuario.EmailJaCadastrado);
            }

            return !_notificador.ExisteMensagem();
        }
    }
}
