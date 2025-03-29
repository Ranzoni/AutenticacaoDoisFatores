using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Validadores;
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

            await _dominio.AlterarAsync(usuarioParaAlterar);

            var usuarioCadastrado = _mapper.Map<UsuarioCadastrado>(usuarioParaAlterar);

            return usuarioCadastrado;
        }

        private async Task<bool> AlteracaoEhValida(Guid id, Usuario? usuarioParaAlterar, NovosDadosUsuario novosDadosUsuario)
        {
            Task<bool>? verificarNomeUsuarioJaCadastrado = null;

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

            if (verificarNomeUsuarioJaCadastrado is not null)
            {
                var existeNomeUsuario = await verificarNomeUsuarioJaCadastrado;
                if (existeNomeUsuario)
                    _notificador.AddMensagem(MensagensValidacaoUsuario.NomeUsuarioJaCadastrado);
            }

            return !_notificador.ExisteMensagem();
        }
    }
}
