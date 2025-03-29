using AutenticacaoDoisFatores.Dominio.Compartilhados;
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
    public class AlterarUsuario(DominioDeUsuarios dominio, IMapper mapper, INotificador notificador)
    {
        private readonly DominioDeUsuarios _dominio = dominio;
        private readonly IMapper _mapper = mapper;
        private readonly INotificador _notificador = notificador;

        public async Task<UsuarioCadastrado?> ExecutarAsync(Guid id, NovosDadosUsuario novosDadosUsuario)
        {
            var usuarioParaAlterar = await _dominio.BuscarUnicoAsync(id);
            if (!await AlteracaoEhValida(id, usuarioParaAlterar, novosDadosUsuario) || usuarioParaAlterar is null)
                return null;

            if (novosDadosUsuario.Nome is not null && !novosDadosUsuario.Nome.EstaVazio())
                usuarioParaAlterar.AlterarNome(novosDadosUsuario.Nome);

            if (novosDadosUsuario.NomeUsuario is not null && !novosDadosUsuario.NomeUsuario.EstaVazio())
                usuarioParaAlterar.AlterarNomeUsuario(novosDadosUsuario.NomeUsuario);

            if (novosDadosUsuario.Email is not null && !novosDadosUsuario.Email.EstaVazio())
                usuarioParaAlterar.AlterarEmail(novosDadosUsuario.Email);

            if (novosDadosUsuario.Senha is not null && !novosDadosUsuario.Senha.EstaVazio())
                usuarioParaAlterar.AlterarSenha(novosDadosUsuario.Senha);

            await _dominio.AlterarAsync(usuarioParaAlterar);

            var usuarioCadastrado = _mapper.Map<UsuarioCadastrado>(usuarioParaAlterar);

            return usuarioCadastrado;
        }

        private async Task<bool> AlteracaoEhValida(Guid id, Usuario? usuarioParaAlterar, NovosDadosUsuario novosDadosUsuario)
        {
            Task<bool>? verificarNomeUsuarioJaCadastrado = null;
            Task<bool>? verificarEmailJaCadastrado = null;

            if (usuarioParaAlterar is null || !usuarioParaAlterar.Ativo || usuarioParaAlterar.EhAdmin)
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);

            if (novosDadosUsuario?.Nome is not null && !ValidadorDeUsuario.NomeEhValido(novosDadosUsuario.Nome))
                _notificador.AddMensagem(MensagensValidacaoUsuario.NomeInvalido);

            if (novosDadosUsuario?.NomeUsuario is not null)
            {
                if (!ValidadorDeUsuario.NomeUsuarioEhValido(novosDadosUsuario.NomeUsuario))
                    _notificador.AddMensagem(MensagensValidacaoUsuario.NomeUsuarioInvalido);
                else 
                    verificarNomeUsuarioJaCadastrado = _dominio.ExisteNomeUsuarioAsync(novosDadosUsuario.NomeUsuario, id);
            }

            if (novosDadosUsuario?.Email is not null)
            {
                if (!ValidadorDeUsuario.EmailEhValido(novosDadosUsuario.Email))
                    _notificador.AddMensagem(MensagensValidacaoUsuario.EmailInvalido);
                else
                    verificarEmailJaCadastrado = _dominio.ExisteEmailAsync(novosDadosUsuario.Email, id);
            }

            if (novosDadosUsuario?.Senha is not null && !Seguranca.ComposicaoSenhaEhValida(novosDadosUsuario.SenhaDescriptografada()))
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
