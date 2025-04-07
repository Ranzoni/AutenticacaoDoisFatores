namespace AutenticacaoDoisFatores.Servico.DTO.Clientes
{
    public class FiltrosParaBuscarClientes : BuscaDeEntidadeComAuditoria
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? NomeDominio { get; set; }
        public bool? Ativo { get; set; }
    }
}
