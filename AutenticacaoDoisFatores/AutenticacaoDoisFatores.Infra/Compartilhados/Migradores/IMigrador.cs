namespace AutenticacaoDoisFatores.Infra.Compartilhados.Migradores
{
    public interface IMigrador
    {
        Task AplicarMigracoesAsync();
        Task AplicarMigracoesAsync(string nomeDominio);
    }
}
