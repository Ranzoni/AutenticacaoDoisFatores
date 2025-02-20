namespace AutenticacaoDoisFatores.Infra.Compartilhados
{
    public sealed class Auditoria
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Acao { get; } = "";
        public object? Detalhes { get; } = null;

        private Auditoria()
        {
        }

        internal Auditoria(string acao, object detalhes)
        {
            Acao = acao;
            Detalhes = detalhes;
        }
    }
}
