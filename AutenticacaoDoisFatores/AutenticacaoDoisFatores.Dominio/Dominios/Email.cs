using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Servicos;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public class Email(IServicoDeEmail servico)
    {
        private readonly IServicoDeEmail _servico = servico;

        public void EnviarConfirmacaoDeCadastroDeCliente(string para, string chaveAcesso, string linkConfirmacao)
        {
            if (para.EstaVazio() || !para.EhEmail())
                ExcecoesEmail.EmailDestinoInvalido();

            var tituloConfirmacaoCadastro = MensagensEnvioEmail.TituloConfirmacaoCadastroCliente.Descricao() ?? "";
            var msgConfirmacaoCadastro = MensagensEnvioEmail.MensagemConfirmacaoCadastroCliente.Descricao() ?? "";
            var textoEstaEhChaveDeAcesso = MensagensEnvioEmail.EstaEhChaveDeAcesso.Descricao() ?? "";
            var textoPararConfirmarCadastroCliente = MensagensEnvioEmail.ParaConfirmarCadastroCliente.Descricao() ?? "";
            var textoLinkConfirmacaoCadastro = MensagensEnvioEmail.TextoDoLinkDeCadastroCliente.Descricao() ?? "";

            var mensagemDoEmail = GerarMensagemEmail
                (
                    mensagem: msgConfirmacaoCadastro,
                    detalhes: @$"
                        <p style='font-size: 18px; color: #666666;'>
                            {textoPararConfirmarCadastroCliente} <a href='{linkConfirmacao}' style='text-decoration: none; background-color: #1e87f0; color: #ffffff; padding: 10px 20px; border-radius: 5px; font-size: 16px;'>{textoLinkConfirmacaoCadastro}</a>
                        </p>
                        <p style='font-size: 18px; color: #666666;'>{textoEstaEhChaveDeAcesso} {chaveAcesso}</p>"
                );

            _servico.Enviar(para: para, titulo: tituloConfirmacaoCadastro, mensagem: mensagemDoEmail);
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
