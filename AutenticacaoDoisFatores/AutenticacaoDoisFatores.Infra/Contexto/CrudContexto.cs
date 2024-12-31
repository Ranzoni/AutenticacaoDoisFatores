using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Infra.Configuracoes;
using Microsoft.EntityFrameworkCore;

namespace AutenticacaoDoisFatores.Infra.Contexto
{
    public class CrudContexto(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClienteConfiguracao());
        }
    }
}
