using AutenticacaoDoisFatores.Domain.Filters;

namespace AutenticacaoDoisFatores.Service.Dtos.Users
{
    public class SearchUserFilters : AuditedEntitySearch
    {
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public long? Phone { get; set; }
        public bool? Active { get; set; }
        public DateTime? LastAccessFrom { get; set; }
        public DateTime? LastAccessUntil { get; set; }

        public static explicit operator UserFilter(SearchUserFilters filter)
        {
            return new UserFilter
            (
                name: filter.Name,
                username: filter.Username,
                email: filter.Email,
                phone: filter.Phone,
                active: filter.Active,
                lastAccessFrom: filter.LastAccessFrom,
                lastAccessUntil: filter.LastAccessUntil,
                isAdmin: false,
                createdFrom: filter.CreatedFrom,
                createdUntil: filter.CreatedUntil,
                updatedFrom: filter.UpdatedFrom,
                updatedUntil: filter.UpdatedUntil,
                page: filter.Page!.Value,
                quantity: filter.Quantity!.Value
            );
        }
    }
}
