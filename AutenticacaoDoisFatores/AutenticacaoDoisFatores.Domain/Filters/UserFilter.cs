namespace AutenticacaoDoisFatores.Domain.Filters
{
    public class UserFilter
    (
        string? name = null,
        string? username = null,
        string? email = null,
        long? phone = null,
        bool? active = null,
        DateTime? lastAccessFrom = null,
        DateTime? lastAccessUntil = null,
        bool? isAdmin = null,
        DateTime? createdFrom = null,
        DateTime? createdUntil = null,
        DateTime? updatedFrom = null,
        DateTime? updatedUntil = null,
        int page = 1,
        int quantity = 10
    ) : AuditedEntityFilter(createdFrom, createdUntil, updatedFrom, updatedUntil, page, quantity)
    {
        public string? Name { get; } = name;
        public string? Username { get; } = username;
        public string? Email { get; } = email;
        public long? Phone { get; } = phone;
        public bool? Active { get; } = active;
        public DateTime? LastAccessFrom { get; } = lastAccessFrom;
        public DateTime? LastAccessUntil { get; } = lastAccessUntil;
        public bool? IsAdmin { get; } = isAdmin;
    }
}
