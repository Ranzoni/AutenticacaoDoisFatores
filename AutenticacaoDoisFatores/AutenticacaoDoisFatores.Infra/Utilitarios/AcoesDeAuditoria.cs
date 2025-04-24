using System.ComponentModel;

namespace AutenticacaoDoisFatores.Infra.Utilitarios
{
    internal enum AcoesDeAuditoria
    {
        [Description("Inclusão")]
        Inclusao,
        [Description("Modificação")]
        Modificacao,
        [Description("Exclusão")]
        Exclusao
    }
}
