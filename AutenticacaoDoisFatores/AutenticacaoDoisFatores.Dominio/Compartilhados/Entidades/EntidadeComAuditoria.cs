namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades
{
    public abstract class EntidadeComAuditoria : EntidadeBase
    {
        private readonly bool _estaCadastrada = false;

        public DateTime DataCadastro { get; protected set; } = DateTime.Now;
        public DateTime? DataAlteracao { get; protected set; }

        protected EntidadeComAuditoria()
        {
            _estaCadastrada = false;
        }

        protected EntidadeComAuditoria(bool estaCadastrada)
        {
            _estaCadastrada = estaCadastrada;
        }

        public bool EntidadeCadastrada()
        {
            return _estaCadastrada;
        }

        public void AtualizarDataAlteracao()
        {
            if (_estaCadastrada)
                DataAlteracao = DateTime.Now;
        }
    }
}
