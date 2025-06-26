using AutenticacaoDoisFatores.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutenticacaoDoisFatores.Infra.Configurations
{
    internal class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(p => p.Id);

            builder
                .Property(p => p.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property(p => p.DomainName)
                .IsRequired()
                .HasMaxLength(100);

            builder
                .HasIndex(p => p.DomainName)
                .IsUnique();

            builder
                .Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder
                .HasIndex(p => p.Email)
                .IsUnique();

            builder
                .Property(p => p.AccessKey)
                .HasMaxLength(2000)
                .IsRequired();

            builder
                .Property(p => p.Active)
                .HasDefaultValue(false);
        }
    }
}
