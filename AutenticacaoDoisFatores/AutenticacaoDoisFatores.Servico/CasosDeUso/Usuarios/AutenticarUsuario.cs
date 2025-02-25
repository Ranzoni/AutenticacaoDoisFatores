using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using AutoMapper;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios
{
    public class AutenticarUsuario(DominioDeUsuarios dominio, INotificador notificador, IMapper mapper)
    {
        private readonly DominioDeUsuarios _dominio = dominio;
        private readonly INotificador _notificador = notificador;
        private readonly IMapper _mapper = mapper;

        public async Task<UsuarioAutenticado?> AutenticarAsync(DadosAutenticacao dadosAutenticacao)
        {
            if (!DadosAutenticaoSaoValidos(dadosAutenticacao))
                return null;

            var usuario = await BuscarUsuarioAsync(dadosAutenticacao);

            if (!AutenticacaoEhValida(usuario, dadosAutenticacao.Senha) || usuario is null)
                return null;

            var token = Seguranca.GerarTokenAutenticacaoUsuario(usuario.Id);

            var usuarioCadastrado = _mapper.Map<UsuarioCadastrado>(usuario);

            var resposta = new UsuarioAutenticado(usuario: usuarioCadastrado, token: token);
            return resposta;
        }

        private bool DadosAutenticaoSaoValidos(DadosAutenticacao dadosAutenticacao)
        {
            var nomeUsuarioVazio = dadosAutenticacao.NomeUsuario is null || dadosAutenticacao.NomeUsuario.EstaVazio();
            var emailVazio = dadosAutenticacao.Email is null || dadosAutenticacao.Email.EstaVazio();

            if (nomeUsuarioVazio && emailVazio)
            {
                _notificador.AddMensagem(MensagensValidacaoUsuario.NomeUsuarioOuEmailObrigatorio);
                return false;
            }

            return true;
        }

        private bool AutenticacaoEhValida(Usuario? usuario, string senha)
        {
            if (usuario is null || !usuario.Ativo)
            {
                _notificador.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.NaoAutenticado);
                return false;
            }

            var senhaCriptografada = Criptografia.CriptografarComSha512(senha);
            if (!usuario.Senha.Equals(senhaCriptografada))
            {
                _notificador.AddMensagemNaoAutorizado(MensagensValidacaoUsuario.NaoAutenticado);
                return false;
            }

            return true;
        }

        private async Task<Usuario?> BuscarUsuarioAsync(DadosAutenticacao dadosAutenticacao)
        {
            if (dadosAutenticacao.NomeUsuario is not null && !dadosAutenticacao.NomeUsuario.EstaVazio())
                return await _dominio.BuscarPorNomeUsuarioAsync(dadosAutenticacao.NomeUsuario);
            else if (dadosAutenticacao.Email is not null && !dadosAutenticacao.Email.EstaVazio())
                return await _dominio.BuscarPorEmailAsync(dadosAutenticacao.Email);

            return null;
        }
    }
}
