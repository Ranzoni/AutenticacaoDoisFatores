namespace AutenticacaoDoisFatores.Domain.Filters
{
    public abstract class AuditedEntityFilter
    (
        DateTime? createdFrom,
        DateTime? createdUntil,
        DateTime? updatedFrom,
        DateTime? updatedUntil,
        int page = 1,
        int quantity = 10
    ) : BaseFilter(page, quantity)
    {
        public DateTime? CreatedFrom { get; } = createdFrom;
        public DateTime? CreatedUntil { get; } = createdUntil;
        public DateTime? UpdatedFrom { get; } = updatedFrom;
        public DateTime? UpdatedUntil { get; } = updatedUntil;
    }
}
