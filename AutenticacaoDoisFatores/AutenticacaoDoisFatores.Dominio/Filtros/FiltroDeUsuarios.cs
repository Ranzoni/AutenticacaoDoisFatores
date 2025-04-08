namespace AutenticacaoDoisFatores.Dominio.Filtros
{
    public class FiltroDeUsuarios
    (
        string? nome = null,
        string? nomeUsuario = null,
        string? email = null,
        bool? ativo = null,
        DateTime? dataUltimoAcessoDe = null,
        DateTime? dataUltimoAcessoAte = null,
        bool? ehAdmin = null,
        DateTime? dataCadastroDe = null,
        DateTime? dataCadastroAte = null,
        DateTime? dataAlteracaoDe = null,
        DateTime? dataAlteracaoAte = null,
        int pagina = 1,
        int qtdPorPagina = 10
    ) : FiltroDeEntidadeComAuditoria(dataCadastroDe, dataCadastroAte, dataAlteracaoDe, dataAlteracaoAte, pagina, qtdPorPagina)
    {
        public string? Nome { get; } = nome;
        public string? NomeUsuario { get; } = nomeUsuario;
        public string? Email { get; } = email;
        public bool? Ativo { get; } = ativo;
        public DateTime? DataUltimoAcessoDe { get; } = dataUltimoAcessoDe;
        public DateTime? DataUltimoAcessoAte { get; } = dataUltimoAcessoAte;
        public bool? EhAdmin { get; } = ehAdmin;
    }
}
