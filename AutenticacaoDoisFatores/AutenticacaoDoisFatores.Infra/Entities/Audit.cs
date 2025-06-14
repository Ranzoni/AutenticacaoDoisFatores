using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Infra.Utils;

namespace AutenticacaoDoisFatores.Infra.Entities
{
    internal class Audit
    {
        internal Guid Id { get; } = Guid.NewGuid();
        internal Guid EntityId { get; } = Guid.Empty;
        internal string Action { get; } = "";
        internal string Table { get; } = "";
        internal object? Details { get; } = null;
        internal DateTime Date { get; } = DateTime.Now;

        private Audit() { }

        internal Audit(AuditActions action, Guid entityId, string table, object details)
        {
            Action = action.Description() ?? "";
            EntityId = entityId;
            Table = table;
            Details = details;
        }
    }
}
