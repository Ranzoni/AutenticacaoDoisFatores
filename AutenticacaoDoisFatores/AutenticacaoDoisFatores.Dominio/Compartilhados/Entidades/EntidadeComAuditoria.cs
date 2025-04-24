namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades
{
    public abstract class EntidadeComAuditoria : EntidadeBase
    {
        private readonly bool _estaCadastrada = false;
        private readonly Queue<AlteracaoAuditada> _alteracoes = [];

        public DateTime DataCadastro { get; protected set; } = DateTime.Now;
        public DateTime? DataAlteracao { get; protected set; }

        private EntidadeComAuditoria()
        {
            _estaCadastrada = false;
        }

        protected EntidadeComAuditoria(bool estaCadastrada = false)
        {
            _estaCadastrada = estaCadastrada;
        }

        public void AuditarModificacao(string campo, string valorAnterior, string valorAtual)
        {
            if (!_estaCadastrada)
                return;

            var campoJaAuditado = _alteracoes.FirstOrDefault(a => a.Campo.ToLower().Equals(campo.ToLower()));
            if (campoJaAuditado is not null)
                campoJaAuditado.ValorAtual = valorAtual;
            else
                _alteracoes.Enqueue(new AlteracaoAuditada(campo, valorAnterior, valorAtual));

            DataAlteracao = DateTime.Now;
        }

        public AlteracaoAuditada[] RetornarAlteracoes()
        {
            return [.. _alteracoes];
        }
    }

    public sealed class AlteracaoAuditada(string campo, string valorAnterior, string valorAtual)
    {
        public string Campo { get; } = campo;
        public string ValorAnterior { get; } = valorAnterior;
        public string ValorAtual { get; set; } = valorAtual;
    }
}
