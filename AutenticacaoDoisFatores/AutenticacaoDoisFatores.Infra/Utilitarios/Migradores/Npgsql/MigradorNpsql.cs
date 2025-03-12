namespace AutenticacaoDoisFatores.Infra.Utilitarios.Migradores.Npgsql
{
    public class MigradorNpsql(string stringDeConexao) : Migrador(stringDeConexao), IMigrador
    {
        public async Task AplicarMigracoesAsync()
        {
            await ExecutarScriptsAsync("Npgsql");
        }

        public async Task AplicarMigracoesAsync(string nomeDominio)
        {
            await ExecutarScriptsAsync(nomeDominio, "Npgsql");
        }
    }
}
