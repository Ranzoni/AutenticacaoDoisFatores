using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Infra.Configurations;
using AutenticacaoDoisFatores.Infra.Entities;
using AutenticacaoDoisFatores.Infra.Utils;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace AutenticacaoDoisFatores.Infra.Contexts
{
    public class BaseContext(DbContextOptions<BaseContext> options) : DbContext(options)
    {
        public DbSet<Client> Clients { get; set; }
        internal DbSet<Audit> Audits { get; set; }

        public List<string> GetSchemas()
        {
            var schemas = new List<string>();

            var sql = @"
                SELECT schema_name 
                FROM information_schema.schemata 
                WHERE schema_name NOT LIKE 'pg_%' 
                AND schema_name != 'information_schema'
                AND schema_name != 'public'
                AND schema_name != 'auxiliar';";

            var resultado = Database.SqlQueryRaw<string>(sql);
            schemas.AddRange(resultado);

            return schemas;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new AuditConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var modifiedEntities = ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged && e.State != EntityState.Detached)
                .ToList();

            foreach (var entity in modifiedEntities)
            {
                Audit audit;

                var primaryKey = entity.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue;

                var modifiedEntity = entity.State == EntityState.Modified;
                if (modifiedEntity)
                {
                    var fields = new ExpandoObject() as IDictionary<string, Object?>;
                    foreach (var prop in entity.Properties)
                    {
                        var fieldName = prop?.Metadata.Name ?? "";
                        var propWasModified = prop?.IsModified ?? false;
                        var originalValue = prop?.OriginalValue ?? "";
                        var currentValue = prop?.CurrentValue ?? "";

                        var fieldWasModified = propWasModified && !originalValue.Equals(currentValue);
                        if (fieldWasModified)
                            fields.Add(fieldName, $"Previous value = '{originalValue}' | New value = '{currentValue}'");
                    }

                    audit = new(action: AuditActions.Change, entityId: (Guid)primaryKey, table: entity.Metadata.Name, details: fields);
                }
                else
                {
                    var action = entity.State == EntityState.Added ? AuditActions.Inclusion : AuditActions.Removal;
                    var fields = new ExpandoObject() as IDictionary<string, Object?>;
                    foreach (var prop in entity.Properties)
                        fields.Add(prop.Metadata.Name, prop.CurrentValue);

                    audit = new(action: action, entityId: (Guid)primaryKey, table: entity.Metadata.Name, details: fields);
                }

                Audits.Add(audit);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
