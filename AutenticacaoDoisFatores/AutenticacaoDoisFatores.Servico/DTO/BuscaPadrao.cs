namespace AutenticacaoDoisFatores.Servico.DTO
{
    public abstract class BuscaPadrao
    {
        public int? Pagina { get; set; } = 1;
        public int? QtdPorPagina { get; set; } = 10;
    }
}
