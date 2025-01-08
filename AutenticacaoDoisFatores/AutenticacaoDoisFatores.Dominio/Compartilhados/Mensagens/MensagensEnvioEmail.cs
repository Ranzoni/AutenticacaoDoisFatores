using System.ComponentModel;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens
{
    public enum MensagensEnvioEmail
    {
        [Description("Confirmação de cadastro de cliente")]
        TituloConfirmacaoCadastroCliente,
        [Description("O seu cadastro no sistema de autenticação de dois fatores foi realizado!<br><br>Para confirmar, clique neste link: <a href='##LINK##'>Confirmar o cadastro!</a>")]
        MsgConfirmacaoCadastroCliente
    }
}
