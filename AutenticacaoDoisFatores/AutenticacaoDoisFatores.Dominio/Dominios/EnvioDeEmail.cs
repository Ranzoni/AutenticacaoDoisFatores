using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Servicos;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public class EnvioDeEmail(IServicoDeEmail servico)
    {
        private readonly IServicoDeEmail _servico = servico;

        public void EnviarAtivacaoDeUsuario(string para, string linkAtivacao)
        {
            if (para.EstaVazio() || !para.EhEmail())
                ExcecoesEmail.EmailDestinoInvalido();

            var tituloAtivacaoUsuario = MensagensEnvioEmail.TituloConfirmacaoCadastroCliente.Descricao() ?? "";
            var msgAtivacaoUsuario = MensagensEnvioEmail.MensagemConfirmacaoCadastroCliente.Descricao() ?? "";
            var textoAtivacaoUsuario = MensagensEnvioEmail.ParaAtivacaoCadastroUsuario.Descricao() ?? "";
            var textoLinkAtivacaoUsuario = MensagensEnvioEmail.TextoDoLinkDeAtivacaoCadastroUsuario.Descricao() ?? "";

            var mensagemDoEmail = GerarMensagemEmail
                (
                    mensagem: msgAtivacaoUsuario,
                    detalhes: @$"
                        <form id='form-confirmar-cadastro'>

                            <p style='font-size: 18px; color: #666666;'>
                                {textoAtivacaoUsuario}
                            </p>

                            <a href='{linkAtivacao}'>
                                {textoLinkAtivacaoUsuario}
                            </a>

                        </form>"
                );

            _servico.Enviar(para: para, titulo: tituloAtivacaoUsuario, mensagem: mensagemDoEmail);
        }

        public void EnviarQrCodeAutenticacaoDoisFatores(string para, string linkParaQrCode, string qrCode)
        {
            var tituloQrCodeAutenticacaoDoisFatores = MensagensEnvioEmail.TituloQrCodeAutenticacaoDoisFatores.Descricao() ?? "";
            var msgQrCodeAutenticacaoDoisFatores = MensagensEnvioEmail.MensagemQrCodeAutenticacaoDoisFatores.Descricao() ?? "";
            var textoLinkParaQrCode = MensagensEnvioEmail.TextoLinkParaQrCode.Descricao() ?? "";

            var mensagemDoEmail = GerarMensagemEmail
                (
                    mensagem: msgQrCodeAutenticacaoDoisFatores,
                    detalhes: @$"
                        <a href='{linkParaQrCode}?qrCode={qrCode}'>
                            {textoLinkParaQrCode}
                        </a>
                    "
                );

            _servico.Enviar(para: para, titulo: tituloQrCodeAutenticacaoDoisFatores, mensagem: mensagemDoEmail);
        }

        public void EnviarCodigoAutenticacaoDoisFatores(string para, string codigo)
        {
            var tituloCodAutenticacaoDoisFatores = MensagensEnvioEmail.TituloCodAutenticacaoDoisFatores.Descricao() ?? "";
            var msgCodAutenticacaoDoisFatores = MensagensEnvioEmail.MensagemCodAutenticacaoDoisFatores.Descricao() ?? "";

            var mensagemDoEmail = GerarMensagemEmail
                (
                    mensagem: msgCodAutenticacaoDoisFatores,
                    detalhes: @$"

                        <p style='font-size: 18px; color: #666666;'>
                            {codigo}
                        </p>
                    "
                );

            _servico.Enviar(para: para, titulo: tituloCodAutenticacaoDoisFatores, mensagem: mensagemDoEmail);
        }

        public void EnviarConfirmacaoDeCadastroDeCliente(string para, string chaveAcesso, string linkConfirmacao, string token)
        {
            if (para.EstaVazio() || !para.EhEmail())
                ExcecoesEmail.EmailDestinoInvalido();

            var tituloConfirmacaoCadastro = MensagensEnvioEmail.TituloConfirmacaoCadastroCliente.Descricao() ?? "";
            var msgConfirmacaoCadastro = MensagensEnvioEmail.MensagemConfirmacaoCadastroCliente.Descricao() ?? "";
            var textoEstaEhChaveDeAcesso = MensagensEnvioEmail.EstaEhChaveDeAcesso.Descricao() ?? "";
            var textoParaConfirmarCadastroCliente = MensagensEnvioEmail.ParaConfirmarChaveCliente.Descricao() ?? "";
            var textoLinkConfirmacaoCadastro = MensagensEnvioEmail.TextoDoLinkDeCadastroCliente.Descricao() ?? "";

            var mensagemDoEmail = GerarMensagemEmail
                (
                    mensagem: msgConfirmacaoCadastro,
                    detalhes: @$"
                        <form id='form-confirmar-cadastro'>

                            <p style='font-size: 18px; color: #666666;'>
                                {textoParaConfirmarCadastroCliente}
                            </p>

                            <a href='{linkConfirmacao}?token={token}'>
                                {textoLinkConfirmacaoCadastro}
                            </a>

                        </form>

                        <p style='font-size: 18px; color: #666666;'>{textoEstaEhChaveDeAcesso}</p>

                        <p style='font-size: 18px; color: #666666;'>{chaveAcesso}</p>"
                );

            _servico.Enviar(para: para, titulo: tituloConfirmacaoCadastro, mensagem: mensagemDoEmail);
        }

        public void EnviarGeracaoDeNovaChaveCliente(string para, string linkConfirmacao, string token)
        {
            if (para.EstaVazio() || !para.EhEmail())
                ExcecoesEmail.EmailDestinoInvalido();

            var tituloConfirmacaoNovaChave = MensagensEnvioEmail.TituloGeracaoNovaChaveCliente.Descricao() ?? "";
            var textoParaGerarNovaChave = MensagensEnvioEmail.ParaGerarNovaChaveCliente.Descricao() ?? "";
            var textoLinkGerarNovaChave = MensagensEnvioEmail.TextoDoLinkDeNovaChaveCliente.Descricao() ?? "";

            var mensagemDoEmail = GerarMensagemEmail
                (
                    mensagem: textoParaGerarNovaChave,
                    detalhes: @$"
                        <form id='form-confirmar-cadastro'>

                            <a href='{linkConfirmacao}?token={token}'>
                                {textoLinkGerarNovaChave}
                            </a>

                        </form>"
                );

            _servico.Enviar(para: para, titulo: tituloConfirmacaoNovaChave, mensagem: mensagemDoEmail);
        }

        public void EnviarConfirmacaoDeNovaChaveCliente(string para, string chaveAcesso)
        {
            if (para.EstaVazio() || !para.EhEmail())
                ExcecoesEmail.EmailDestinoInvalido();

            var tituloConfirmacaoNovaChaveCliente = MensagensEnvioEmail.TituloConfirmacaoNovaChaveCliente.Descricao() ?? "";
            var msgConfirmacaoNovaChaveCliente = MensagensEnvioEmail.MensagemConfirmacaoNovaChaveCliente.Descricao() ?? "";
            var textoEstaEhChaveDeAcesso = MensagensEnvioEmail.EstaEhChaveDeAcesso.Descricao() ?? "";

            var mensagemDoEmail = GerarMensagemEmail
                (
                    mensagem: msgConfirmacaoNovaChaveCliente,
                    detalhes: @$"
                        <p style='font-size: 18px; color: #666666;'>{textoEstaEhChaveDeAcesso}</p>

                        <p style='font-size: 18px; color: #666666;'>{chaveAcesso}</p>"
                );

            _servico.Enviar(para: para, titulo: tituloConfirmacaoNovaChaveCliente, mensagem: mensagemDoEmail);
        }

        private static string GerarMensagemEmail(string mensagem, string detalhes)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                    <body>
                        <div class='email-container' style='width: 100%; max-width: 600px; margin: 0 auto; background-color: #ffffff; box-shadow: 0px 0px 10px 0px rgba(0,0,0,0.1); padding: 20px; border-radius: 8px;'>
                            <div style='text-align: center; padding: 10px 0;'>
                                <h1 style='color: #333333;'>{mensagem}</h1>
                            </div>

                            <div style='text-align: center; padding: 20px 0;'>
                                {detalhes}
                            </div>

                            <div style='text-align: center; margin-top: 30px; font-size: 12px; color: #999999'>
                                <p>© 2025 Autenticação em Dois Fatores. Todos os direitos reservados.</p>
                            </div>
                        </div>
                    </body>
                </html>";
        }
    }
}
