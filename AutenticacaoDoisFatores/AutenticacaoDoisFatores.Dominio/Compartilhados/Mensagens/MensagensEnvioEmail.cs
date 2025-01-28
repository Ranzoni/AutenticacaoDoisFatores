using System.ComponentModel;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens
{
    public enum MensagensEnvioEmail
    {
        [Description("Confirmação de cadastro de cliente")]
        TituloConfirmacaoCadastroCliente,
        [Description("O seu cadastro no sistema de autenticação de dois fatores foi realizado!")]
        MensagemConfirmacaoCadastroCliente,
        [Description("Esta é a sua chave de acesso:")]
        EstaEhChaveDeAcesso,
        [Description("Para confirmar, clique neste link:")]
        ParaConfirmarCadastroCliente,
        [Description("Confirmar o cadastro!")]
        TextoDoLinkDeCadastroCliente,
        [Description("Confirmação de recuperação da chave de acesso")]
        TituloConfirmacaoNovaChaveCliente,
        [Description("Para gerar a nova chave, clique neste link:")]
        ParaGerarNovaChaveCliente,
        [Description("Gerar nova chave!")]
        TextoDoLinkDeNovaChaveCliente
    }
}
