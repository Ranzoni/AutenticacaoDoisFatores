namespace AutenticacaoDoisFatores.Infra.Compartilhados.Migradores.Npgsql
{
    public class MigradorNpsql(string stringDeConexao) : Migrador(stringDeConexao), IMigrador
    {
        public void AplicarMigracoes()
        {
            AplicarMigracoes("Npgsql");
        }
    }
}
