namespace AutenticacaoDoisFatores.Dominio.Filtros
{
    public class FiltroDeClientes
    (
        string? nome = null,
        string? email = null,
        string? nomeDominio = null,
        bool? ativo = null,
        DateTime? dataCadastroDe = null,
        DateTime? dataCadastroAte = null,
        DateTime? dataAlteracaoDe = null,
        DateTime? dataAlteracaoAte = null,
        int pagina = 1,
        int qtdPorPagina = 10
    ) : FiltroDeEntidadeComAuditoria(dataCadastroDe, dataCadastroAte, dataAlteracaoDe, dataAlteracaoAte, pagina, qtdPorPagina)
    {
        public string? Nome { get; } = nome;
        public string? Email { get; } = email;
        public string? NomeDominio { get; } = nomeDominio;
        public bool? Ativo { get; } = ativo;
    }
}
