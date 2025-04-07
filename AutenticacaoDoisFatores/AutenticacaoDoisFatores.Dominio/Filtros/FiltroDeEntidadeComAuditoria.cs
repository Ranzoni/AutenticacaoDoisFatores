namespace AutenticacaoDoisFatores.Dominio.Filtros
{
    public abstract class FiltroDeEntidadeComAuditoria
    (
        DateTime? dataCadastroDe,
        DateTime? dataCadastroAte,
        DateTime? dataAlteracaoDe,
        DateTime? dataAlteracaoAte,
        int pagina = 1,
        int qtdPorPagina = 10
    ) : FiltroPadrao(pagina, qtdPorPagina)
    {
        public DateTime? DataCadastroDe { get; } = dataCadastroDe;
        public DateTime? DataCadastroAte { get; } = dataCadastroAte;
        public DateTime? DataAlteracaoDe { get; } = dataAlteracaoDe;
        public DateTime? DataAlteracaoAte { get; } = dataAlteracaoAte;
    }
}
