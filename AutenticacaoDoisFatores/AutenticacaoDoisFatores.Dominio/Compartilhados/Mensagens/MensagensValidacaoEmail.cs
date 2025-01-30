using System.ComponentModel;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens
{
    public enum MensagensValidacaoEmail
    {
        [Description("O endereço de e-mail de destino não foi informado ou está inválido.")]
        EmailDestinoNaoInvalido,
        [Description("O título do e-mail não foi informado.")]
        TituloEmailNaoInformado,
        [Description("A mensagem do e-mail não foi informada.")]
        MensagemEmailNaoInformada,
        [Description("O endereço de e-mail de envio não foi configurado.")]
        EnderecoEmailEnvioNaoConfigurado
    }
}
