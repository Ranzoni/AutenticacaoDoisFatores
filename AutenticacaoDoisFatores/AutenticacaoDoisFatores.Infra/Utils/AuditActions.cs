using System.ComponentModel;

namespace AutenticacaoDoisFatores.Infra.Utils
{
    internal enum AuditActions
    {
        [Description("Inclusão")]
        Inclusion,
        [Description("Modificação")]
        Change,
        [Description("Exclusão")]
        Removal
    }
}
