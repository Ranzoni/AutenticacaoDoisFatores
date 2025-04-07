namespace AutenticacaoDoisFatores.Dominio.Filtros
{
    public abstract class FiltroPadrao(int pagina, int qtdPorPagina)
    {
        public int Pagina { get; } = pagina;
        public int QtdPorPagina { get; } = qtdPorPagina;
    }
}
