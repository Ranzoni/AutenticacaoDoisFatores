using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Infra.Compartilhados;
using AutenticacaoDoisFatores.Infra.Configuracoes;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace AutenticacaoDoisFatores.Infra.Contexto
{
    public class ContextoPadrao(DbContextOptions<ContextoPadrao> opcoes) : DbContext(opcoes)
    {
        public DbSet<Cliente> Clientes { get; set; }
        internal DbSet<Auditoria> Auditorias { get; set; }

        public List<string> RetornarSchemas()
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
            modelBuilder.ApplyConfiguration(new ClienteConfiguracao());
            modelBuilder.ApplyConfiguration(new AuditoriaConfiguracao());

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entidadesParaAlteracao = ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged && e.State != EntityState.Detached)
                .ToList();

            foreach (var entidadeEmAlteracao in entidadesParaAlteracao)
            {
                Auditoria auditoria;

                var entidadeSeraModificada = entidadeEmAlteracao.State == EntityState.Modified;
                if (entidadeSeraModificada)
                {
                    var campos = new ExpandoObject() as IDictionary<string, Object?>;
                    foreach (var propriedade in entidadeEmAlteracao.Properties)
                    {
                        var nomeDoCampo = propriedade?.Metadata.Name ?? "";
                        var propTemAlteracao = propriedade?.IsModified ?? false;
                        var valorOriginal = propriedade?.OriginalValue ?? "";
                        var valorAtual = propriedade?.CurrentValue ?? "";

                        var campoFoiModificado = propTemAlteracao && !valorOriginal.Equals(valorAtual);
                        if (campoFoiModificado)
                            campos.Add(nomeDoCampo, $"Valor antigo = '{valorOriginal}' | Valor novo = '{valorAtual}'");
                    }

                    auditoria = new(acao: "Modificação", detalhes: campos);
                }
                else
                {
                    var acao = entidadeEmAlteracao.State == EntityState.Added ? "Inclusão" : "Remoção";
                    var campos = new ExpandoObject() as IDictionary<string, Object?>;
                    foreach (var propriedade in entidadeEmAlteracao.Properties)
                        campos.Add(propriedade.Metadata.Name, propriedade.CurrentValue);

                    auditoria = new(acao: acao, detalhes: campos);
                }

                Auditorias.Add(auditoria);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
