using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using Mensageiro;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores;
using Microsoft.Extensions.DependencyInjection;
using AutenticacaoDoisFatores.Dominio.Validadores;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios
{
    public class AutenticarUsuario(INotificador notificador, DominioDeUsuarios dominio, IServiceProvider provedorDeServicos)
    {
        private readonly INotificador _notificador = notificador;
        private readonly DominioDeUsuarios _dominio = dominio;
        private readonly IServiceProvider _provedorDeServicos = provedorDeServicos;

        public async Task<object?> ExecutarAsync(DadosAutenticacao dadosAutenticacao)
        {
            if (!DadosAutenticacaoSaoValidos(dadosAutenticacao))
                return null;

            var usuario = await BuscarUsuarioAsync(dadosAutenticacao);
            if (!AutenticacaoEhValida(usuario, dadosAutenticacao.Senha))
                return null;

            var autenticador = RetornarTipoDeAutenticador(usuario!);
            return await autenticador.ExecutarAsync(usuario!);
        }

        private bool DadosAutenticacaoSaoValidos(DadosAutenticacao dadosAutenticacao)
        {
            var nomeUsuarioVazio = dadosAutenticacao.NomeUsuarioOuEmail is null || dadosAutenticacao.NomeUsuarioOuEmail.EstaVazio();

            if (nomeUsuarioVazio)
            {
                _notificador.AddMensagem(MensagensValidacaoUsuario.NomeUsuarioOuEmailObrigatorio);
                return false;
            }

            return true;
        }

        private async Task<Usuario?> BuscarUsuarioAsync(DadosAutenticacao dadosAutenticacao)
        {
            if (ValidadorDeUsuario.EmailEhValido(dadosAutenticacao.NomeUsuarioOuEmail))
                return await _dominio.BuscarPorEmailAsync(dadosAutenticacao.NomeUsuarioOuEmail);
            else 
                return await _dominio.BuscarPorNomeUsuarioAsync(dadosAutenticacao.NomeUsuarioOuEmail);
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

        private ITipoDeAutenticador RetornarTipoDeAutenticador(Usuario usuario)
        {
            if (usuario.ExisteTipoDeAutenticacaoConfigurado())
                return _provedorDeServicos.GetRequiredService<AutenticadorUsuarioEmDoisFatores>();
            else
                return _provedorDeServicos.GetRequiredService<AutenticadorUsuarioPadrao>();
        }
    }
}
