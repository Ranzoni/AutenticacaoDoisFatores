namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades
{
    public abstract class EntidadeComAuditoria : EntidadeBase
    {
        public DateTime DataCadastro { get; protected set; } = DateTime.Now;
        public DateTime? DataAlteracao { get; protected set; }
    }
}
