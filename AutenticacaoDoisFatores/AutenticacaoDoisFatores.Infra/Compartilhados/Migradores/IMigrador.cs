namespace AutenticacaoDoisFatores.Infra.Compartilhados.Migradores
{
    public interface IMigrador
    {
        void AplicarMigracoes(string stringDeConexao);
    }
}
