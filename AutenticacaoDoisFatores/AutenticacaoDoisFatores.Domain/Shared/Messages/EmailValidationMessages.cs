using System.ComponentModel;

namespace AutenticacaoDoisFatores.Domain.Shared.Messages
{
    public enum EmailValidationMessages
    {
        [Description("O endereço de e-mail de destino não foi informado ou está inválido.")]
        InvalidDestinationEmail,
        [Description("O título do e-mail não foi informado.")]
        EmptySubject,
        [Description("A mensagem do e-mail não foi informada.")]
        EmptyMessage,
        [Description("O endereço de e-mail de envio não foi configurado.")]
        SendingEmailNotFound
    }
}
