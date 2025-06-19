using System.ComponentModel;

namespace AutenticacaoDoisFatores.Domain.Shared.Messages
{
    public enum EmailMessages
    {
        [Description("Confirmação de cadastro de cliente")]
        ClientConfirmationSubject,
        [Description("O seu cadastro no sistema de autenticação de dois fatores foi realizado!")]
        ClientConfirmationMessage,
        [Description("Esta é a sua chave de acesso:")]
        ThisIsYoursAccessKey,
        [Description("Para confirmar, clique neste link:")]
        ToConfirmAccessKeyMessage,
        [Description("Confirmar o cadastro!")]
        ClientConfirmationLinkMessage,
        [Description("Recuperação da chave de acesso")]
        NewAccessKeySubject,
        [Description("Para gerar a nova chave, clique neste link:")]
        ToNewAccessKeyMessage,
        [Description("Gerar nova chave!")]
        NewAccessKeyLinkMessage,
        [Description("Confirmação de nova chave de acesso")]
        NewAccessKeyConfirmationSubject,
        [Description("A sua chave de acesso foi alterada.")]
        NewAccessKeyConfirmationMessage,
        [Description("Código de acesso ao sistema")]
        TwoFactorAuthTokenSubject,
        [Description("Utilize o código abaixo para acessar o sistema.")]
        TwoFactorAuthTokenMessage,
        [Description("Confirmação de Login")]
        TwoFactorAuthCodeSubject,
        [Description("Clique no link abaixo para gerar o QR Code e use um aplicativo de autenticação de sua preferência para escanea-lo.")]
        TwoFactorAuthCodeMessage,
        [Description("Gerar QR Code")]
        GenerateQrCodeMessage
    }
}
