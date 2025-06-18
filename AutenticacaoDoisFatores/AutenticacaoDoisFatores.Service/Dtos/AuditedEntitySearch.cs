namespace AutenticacaoDoisFatores.Service.Dtos
{
    public class AuditedEntitySearch : BaseSearch
    {
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedUntil { get; set; }
        public DateTime? UpdatedFrom { get; set; }
        public DateTime? UpdatedUntil { get; set; }
    }
}
