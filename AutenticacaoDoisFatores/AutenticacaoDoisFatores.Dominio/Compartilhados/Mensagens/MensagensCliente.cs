using System.ComponentModel;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens
{
    internal enum MensagensCliente
    {
        [Description("O nome do cliente não foi preenchido.")]
        NomeNaoPreenchido,
        [Description("O e-mail do cliente não foi preenchido.")]
        EmailNaoPreenchido
    }
}
