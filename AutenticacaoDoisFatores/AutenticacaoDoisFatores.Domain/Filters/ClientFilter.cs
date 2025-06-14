namespace AutenticacaoDoisFatores.Domain.Filters
{
    public class ClientFilter
    (
        string? name = null,
        string? email = null,
        string? domainName = null,
        bool? active = null,
        DateTime? createdFrom = null,
        DateTime? createdUntil = null,
        DateTime? updatedFrom = null,
        DateTime? updatedUntil = null,
        int page = 1,
        int quantity = 10
    ) : AuditedEntityFilter(createdFrom, createdUntil, updatedFrom, updatedUntil, page, quantity)
    {
        public string? Name { get; } = name;
        public string? Email { get; } = email;
        public string? DomainName { get; } = domainName;
        public bool? Active { get; } = active;
    }
}
