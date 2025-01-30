namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades
{
    public abstract class EntidadeBase
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
    }
}
