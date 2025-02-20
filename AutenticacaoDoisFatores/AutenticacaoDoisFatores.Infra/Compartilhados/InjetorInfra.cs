using AutenticacaoDoisFatores.Infra.Compartilhados.Migradores;
using AutenticacaoDoisFatores.Infra.Compartilhados.Migradores.Npgsql;

namespace AutenticacaoDoisFatores.Infra.Compartilhados
{
    public static class InjetorInfra
    {
        public static IMigrador RetornarMigrador()
        {
            return new MigradorNpsql();
        }
    }
}
