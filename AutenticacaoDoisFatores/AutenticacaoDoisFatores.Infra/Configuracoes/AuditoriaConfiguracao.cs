using AutenticacaoDoisFatores.Infra.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

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

        internal static void Configurar()
        {
            BsonClassMap.RegisterClassMap<Auditoria>(map =>
            {
                map.AutoMap();

                map.MapIdMember(a => a.Id)
                    .SetSerializer(new GuidSerializer(BsonType.String))
                    .SetElementName("_id");

                map.MapMember(a => a.IdEntidade)
                    .SetSerializer(new GuidSerializer(BsonType.String))
                    .SetElementName("idEntidade");

                map.MapMember(a => a.Acao)
                    .SetElementName("acao");

                map.MapMember(a => a.Tabela)
                    .SetElementName("tabela");

                map.MapMember(a => a.Detalhes)
                    .SetSerializer(new ObjectSerializer(type => true))
                    .SetElementName("detalhes");

                map.MapMember(c => c.Data)
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Local))
                    .SetElementName("data");
            });
        }
    }
}
