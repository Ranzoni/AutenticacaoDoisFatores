namespace AutenticacaoDoisFatores.Infra.Utils.Migrants
{
    public interface IMigration
    {
        Task ApplyMigrationsAsync();
        Task ApplyMigrationsAsync(string domainName);
    }
}
