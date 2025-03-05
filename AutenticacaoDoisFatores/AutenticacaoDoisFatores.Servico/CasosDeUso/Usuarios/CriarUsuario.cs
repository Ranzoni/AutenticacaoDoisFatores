using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Validadores;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using AutoMapper;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios
{
    public class CriarUsuario(IMapper mapeador, INotificador notificador, DominioDeUsuarios dominio)
    {
        private readonly IMapper _mapeador = mapeador;
        private readonly INotificador _notificador = notificador;
        private readonly DominioDeUsuarios _dominio = dominio;

        public async Task<UsuarioCadastrado?> CriarAsync(NovoUsuario novoUsuario)
        {
            var criacaoEhValida = await CriacaoEhValida(novoUsuario);
            if (!criacaoEhValida)
                return null;

            var usuario = _mapeador.Map<Usuario>(novoUsuario);

            await _dominio.CriarUsuarioAsync(usuario);

            var usuarioCriado = _mapeador.Map<UsuarioCadastrado>(usuario);

            return usuarioCriado;
        }

        private async Task<bool> CriacaoEhValida(NovoUsuario novoUsuario)
        {
            Task<bool>? verificarNomeUsuarioJaCadastrado = null;
            Task<bool>? verificarEmailJaCadastrado = null;

            if (!ValidadorDeUsuario.NomeEhValido(novoUsuario.Nome))
                _notificador.AddMensagem(MensagensValidacaoUsuario.NomeInvalido);

            if (!ValidadorDeUsuario.NomeUsuarioEhValido(novoUsuario.NomeUsuario))
                _notificador.AddMensagem(MensagensValidacaoUsuario.NomeUsuarioInvalido);
            else
                verificarNomeUsuarioJaCadastrado = _dominio.ExisteNomeUsuarioAsync(novoUsuario.NomeUsuario);

            if (!ValidadorDeUsuario.EmailEhValido(novoUsuario.Email))
                _notificador.AddMensagem(MensagensValidacaoUsuario.EmailInvalido);
            else
                verificarEmailJaCadastrado = _dominio.ExisteEmailAsync(novoUsuario.Email);

            if (!Seguranca.ComposicaoSenhaEhValida(novoUsuario.SenhaDescriptografada()))
                _notificador.AddMensagem(MensagensValidacaoUsuario.SenhaInvalida);

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
