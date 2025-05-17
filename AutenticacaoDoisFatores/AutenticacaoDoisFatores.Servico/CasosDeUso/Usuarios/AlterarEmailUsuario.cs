using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Validadores;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios
{
    public class AlterarEmailUsuario(DominioDeUsuarios dominio, INotificador notificador)
    {
        private readonly DominioDeUsuarios _dominio = dominio;
        private readonly INotificador _notificador = notificador;

        public async Task ExecutarAsync(Guid idUsuario, TrocarEmailUsuario trocarEmailUsuario)
        {
            var usuario = await _dominio.BuscarUnicoAsync(idUsuario);
            if (!await AlteracaoEhValidaAsync(usuario, trocarEmailUsuario))
                return;

            usuario!.AlterarEmail(trocarEmailUsuario.NovoEmail);
            await _dominio.AlterarAsync(usuario);
        }

        private async Task<bool> AlteracaoEhValidaAsync(Usuario? usuario, TrocarEmailUsuario trocarEmailUsuario)
        {
            if (usuario is null || !usuario.Ativo || usuario.EhAdmin)
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);

            if (!ValidadorDeUsuario.EmailEhValido(trocarEmailUsuario.NovoEmail))
                _notificador.AddMensagem(MensagensValidacaoUsuario.EmailInvalido);
            else
            {
                var emailJaCadastrado = await _dominio.ExisteEmailAsync(trocarEmailUsuario.NovoEmail);
                if (emailJaCadastrado)
                    _notificador.AddMensagem(MensagensValidacaoUsuario.EmailJaCadastrado);
            }

            return !_notificador.ExisteMensagem();
        }
    }
}
