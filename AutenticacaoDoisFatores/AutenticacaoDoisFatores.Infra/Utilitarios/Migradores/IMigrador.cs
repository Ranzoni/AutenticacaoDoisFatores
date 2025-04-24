namespace AutenticacaoDoisFatores.Infra.Utilitarios.Migradores
{
    public interface IMigrador
    {
        Task AplicarMigracoesAsync();
        Task AplicarMigracoesAsync(string nomeDominio);
    }
}
