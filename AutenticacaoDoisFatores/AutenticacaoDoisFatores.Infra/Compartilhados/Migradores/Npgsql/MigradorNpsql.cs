namespace AutenticacaoDoisFatores.Infra.Compartilhados.Migradores.Npgsql
{
    internal class MigradorNpsql : Migrador, IMigrador
    {
        public void AplicarMigracoes(string stringDeConexao)
        {
            AplicarMigracoes(stringDeConexao, "Npgsql");
        }
    }
}
