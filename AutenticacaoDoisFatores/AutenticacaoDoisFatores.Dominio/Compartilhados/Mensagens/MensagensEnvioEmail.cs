using System.ComponentModel;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens
{
    public enum MensagensEnvioEmail
    {
        [Description("Confirmação de cadastro de cliente")]
        TituloConfirmacaoCadastroCliente,
        [Description("O seu cadastro no sistema de autenticação de dois fatores foi realizado!")]
        MensagemConfirmacaoCadastroCliente,
        [Description("Para confirmar, clique neste link:")]
        ParaConfirmarCadastroCliente,
        [Description("Confirmar o cadastro!")]
        TextoDoLinkDeCadastroCliente
    }
}
