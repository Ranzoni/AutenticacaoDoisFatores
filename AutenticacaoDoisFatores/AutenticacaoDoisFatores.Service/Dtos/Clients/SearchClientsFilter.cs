using AutenticacaoDoisFatores.Domain.Filters;

namespace AutenticacaoDoisFatores.Service.Dtos.Clients
{
    public class SearchClientsFilter : AuditedEntitySearch
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? DomainName { get; set; }
        public bool? Active { get; set; }

        public static explicit operator ClientFilter(SearchClientsFilter filter)
        {
            return new ClientFilter
            (
                name: filter.Name,
                email: filter.Email,
                domainName: filter.DomainName,
                active: filter.Active,
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
