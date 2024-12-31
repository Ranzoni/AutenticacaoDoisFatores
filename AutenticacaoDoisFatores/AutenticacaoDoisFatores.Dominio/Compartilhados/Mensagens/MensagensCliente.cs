using System.ComponentModel;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens
{
    public enum MensagensCliente
    {
        [Description("O nome do cliente não foi preenchido.")]
        NomeInvalido,
        [Description("O e-mail do cliente não foi preenchido.")]
        EmailInvalido,
        [Description("O cliente não foi encontrado.")]
        ClienteNaoEncontrado
    }
}
