namespace AutenticacaoDoisFatores.Infra.Compartilhados
{
    public sealed class Auditoria
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Guid IdEntidade { get; } = Guid.Empty;
        public string Acao { get; } = "";
        public string Tabela { get; } = "";
        public object? Detalhes { get; } = null;
        public DateTime Data { get; } = DateTime.Now;

        private Auditoria()
        {
        }

        internal Auditoria(string acao, Guid idEntidade, string tabela, object detalhes)
        {
            Acao = acao;
            IdEntidade = idEntidade;
            Tabela = tabela;
            Detalhes = detalhes;
        }
    }
}
