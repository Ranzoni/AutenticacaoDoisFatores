namespace AutenticacaoDoisFatores.Service.Dtos
{
    public abstract class BaseSearch
    {
        public int? Page { get; set; } = 1;
        public int? Quantity { get; set; } = 10;
    }
}
