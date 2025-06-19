using AutenticacaoDoisFatores.Infra.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace AutenticacaoDoisFatores.Infra.Configurations
{
    internal class PermissionConfiguration
    {
        internal static void Configure()
        {
            BsonClassMap.RegisterClassMap<Permission>(map =>
            {
                map.AutoMap();

                map.MapIdMember(c => c.Id)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard))
                    .SetElementName("_id");

                map.MapMember(c => c.UserId)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard))
                    .SetElementName("userId");

                map.MapMember(c => c.Permissions)
                    .SetElementName("permissions");

                map.MapMember(c => c.CreatedAt)
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Local))
                    .SetElementName("createdAt");

                map.MapMember(c => c.UpdatedAt)
                    .SetSerializer(new NullableSerializer<DateTime>(new DateTimeSerializer(DateTimeKind.Utc)))
                    .SetElementName("updatedAt");
            });
        }
    }
}
