namespace AutenticacaoDoisFatores.Infra.Utils.Migrants.Npgsql
{
    public class NpsqlMigration(string connectionString) : Migration(connectionString), IMigration
    {
        public async Task ApplyMigrationsAsync()
        {
            await ExecuteScriptsAsync("Npgsql");
        }

        public async Task ApplyMigrationsAsync(string domainName)
        {
            await ExecuteScriptsAsync(domainName, "Npgsql");
        }
    }
}
