namespace AutenticacaoDoisFatores.Servico.DTO
{
    public class BuscaDeEntidadeComAuditoria
    {
        public DateTime? DataCadastroDe { get; set; }
        public DateTime? DataCadastroAte { get; set; }
        public DateTime? DataAlteracaoDe { get; set; }
        public DateTime? DataAlteracaoAte { get; set; }
    }
}
