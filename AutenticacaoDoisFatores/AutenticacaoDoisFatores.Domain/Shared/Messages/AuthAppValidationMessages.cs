using System.ComponentModel;

namespace AutenticacaoDoisFatores.Domain.Shared.Messages
{
    public enum AuthAppValidationMessages
    {
        [Description("A chave secreta para geração de QR Code não foi encontrada.")]
        SecretKeyNotFound,
        [Description("O código para autenticação não foi informado.")]
        CodeNotInformed,
        [Description("Não foi possível gerar o QR Code.")]
        QrCodeNotGenerated
    }
}
