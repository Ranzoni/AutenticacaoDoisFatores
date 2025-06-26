using AutenticacaoDoisFatores.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace AutenticacaoDoisFatores.Infra.Configurations
{
    internal class AuditConfiguration : IEntityTypeConfiguration<Audit>
    {
        public void Configure(EntityTypeBuilder<Audit> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Action)
                .HasColumnType("varchar(256)")
                .IsRequired();

            builder.Property(a => a.EntityId)
                .HasColumnType("uuid")
                .IsRequired();

            builder.Property(a => a.Table)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(a => a.Details)
                .HasColumnType("jsonb");

            builder.Property(a => a.Date)
                .HasColumnType("timestamp")
                .IsRequired();
        }

        internal static void Configurar()
        {
            BsonClassMap.RegisterClassMap<Audit>(map =>
            {
                map.AutoMap();

                map.MapIdMember(a => a.Id)
                    .SetSerializer(new GuidSerializer(BsonType.String))
                    .SetElementName("_id");

                map.MapMember(a => a.EntityId)
                    .SetSerializer(new GuidSerializer(BsonType.String))
                    .SetElementName("entityId");

                map.MapMember(a => a.Action)
                    .SetElementName("action");

                map.MapMember(a => a.Table)
                    .SetElementName("table");

                map.MapMember(a => a.Details)
                    .SetSerializer(new ObjectSerializer(type => true))
                    .SetElementName("details");

                map.MapMember(c => c.Date)
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc))
                    .SetElementName("date");
            });
        }
    }
}
