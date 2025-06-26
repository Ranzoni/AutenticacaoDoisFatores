namespace AutenticacaoDoisFatores.Domain.Shared.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
    }
}
