namespace AutenticacaoDoisFatores.Domain.Filters
{
    public abstract class BaseFilter(int page, int quantity)
    {
        public int Page { get; } = page;
        public int Quantity { get; } = quantity;
    }
}
