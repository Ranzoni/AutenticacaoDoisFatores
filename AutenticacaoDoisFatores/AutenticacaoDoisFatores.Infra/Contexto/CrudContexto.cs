using AutenticacaoDoisFatores.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace AutenticacaoDoisFatores.Infra.Contexto
{
    public class CrudContexto : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
    }
}
