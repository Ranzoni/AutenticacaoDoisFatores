using AutenticacaoDoisFatores.Infra.Compartilhados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutenticacaoDoisFatores.Infra.Configuracoes
{
    internal class AuditoriaConfiguracao : IEntityTypeConfiguration<Auditoria>
    {
        public void Configure(EntityTypeBuilder<Auditoria> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Acao)
                .HasColumnType("varchar(256)")
                .IsRequired();

            builder.Property(a => a.IdEntidade)
                .HasColumnType("uuid")
                .IsRequired();

            builder.Property(a => a.Tabela)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(a => a.Detalhes)
                .HasColumnType("jsonb");

            builder.Property(a => a.Data)
                .HasColumnType("timestamp")
                .IsRequired();
        }
    }
}
