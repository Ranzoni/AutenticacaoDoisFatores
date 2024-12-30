using AutenticacaoDoisFatores.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutenticacaoDoisFatores.Infra.Configuracoes
{
    public class ClienteConfiguracao : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.HasKey(p => p.Id);

            builder
                .Property(p => p.Nome)
                .HasMaxLength(50)
                .IsRequired(true);

            builder
                .Property("Nome_Schema")
                .IsRequired()
                .HasMaxLength(100);

            builder
                .HasIndex("Nome_Schema")
                .IsUnique();

            builder
                .Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(60);

            builder
                .HasIndex(p => p.Email)
                .IsUnique();
        }
    }
}
