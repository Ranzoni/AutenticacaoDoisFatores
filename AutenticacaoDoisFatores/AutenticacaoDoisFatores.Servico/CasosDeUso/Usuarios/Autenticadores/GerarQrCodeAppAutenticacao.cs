using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores
{
    public class GerarQrCodeAppAutenticacao(AppAutenticador appAutenticador, DominioDeUsuarios usuarios, EnvioDeEmail email, INotificador notificador, string linkBaseParaQrCode)
    {
        private readonly DominioDeUsuarios _usuarios = usuarios;
        private readonly AppAutenticador _appAutenticador = appAutenticador;
        private readonly EnvioDeEmail _email = email;
        private readonly INotificador _notificador = notificador;

        public async Task ExecutarAsync(Guid idUsuario)
        {
            var usuario = await _usuarios.BuscarUnicoAsync(idUsuario);
            if (!GeracaoEhValida(usuario))
                return;

            var qrCode = _appAutenticador.GerarQrCode(usuario!);
            if (!QrCodeEhValido(qrCode))
                return;

            _email.EnviarQrCodeAutenticacaoDoisFatores(usuario!.Email, linkBaseParaQrCode, qrCode!);
        }

        private bool GeracaoEhValida(Usuario? usuario)
        {
            if (usuario is null || !usuario.Ativo)
            {
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);
                return false;
            }

            return true;
        }

        private bool QrCodeEhValido(string? qrCode)
        {
            if (qrCode is null || qrCode.EstaVazio())
            {
                _notificador.AddMensagem(MensagensValidacaoAppAutenticacao.QrCodeNaoGerado);
                return false;
            }

            return true;
        }
    }
}
