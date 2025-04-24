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
        ParaConfirmarChaveCliente,
        [Description("Confirmar o cadastro!")]
        TextoDoLinkDeCadastroCliente,
        [Description("Recuperação da chave de acesso")]
        TituloGeracaoNovaChaveCliente,
        [Description("Para gerar a nova chave, clique neste link:")]
        ParaGerarNovaChaveCliente,
        [Description("Gerar nova chave!")]
        TextoDoLinkDeNovaChaveCliente,
        [Description("Confirmação de nova chave de acesso")]
        TituloConfirmacaoNovaChaveCliente,
        [Description("A sua chave de acesso foi alterada.")]
        MensagemConfirmacaoNovaChaveCliente,
        [Description("Código de acesso ao sistema")]
        TituloCodAutenticacaoDoisFatores,
        [Description("Utilize o código abaixo para acessar o sistema.")]
        MensagemCodAutenticacaoDoisFatores,
        [Description("Confirmação de Login")]
        TituloQrCodeAutenticacaoDoisFatores,
        [Description("Clique no link abaixo para gerar o QR Code e use um aplicativo de autenticação de sua preferência para escanea-lo.")]
        MensagemQrCodeAutenticacaoDoisFatores,
        [Description("Gerar QR Code")]
        TextoLinkParaQrCode
    }
}
