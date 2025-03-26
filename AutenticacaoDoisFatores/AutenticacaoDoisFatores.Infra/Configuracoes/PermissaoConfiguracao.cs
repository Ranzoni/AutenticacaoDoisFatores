using AutenticacaoDoisFatores.Infra.Entidades;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace AutenticacaoDoisFatores.Infra.Configuracoes
{
    internal class PermissaoConfiguracao
    {
        internal static void Configurar()
        {
            BsonClassMap.RegisterClassMap<Permissao>(map =>
            {
                map.AutoMap();

                map.MapIdMember(c => c.Id)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard))
                    .SetElementName("_id");

                map.MapMember(c => c.IdUsuario)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard))
                    .SetElementName("idUsuario");

                map.MapMember(c => c.Permissoes)
                    .SetElementName("permissoes");

                map.MapMember(c => c.DataCadastro)
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Local))
                    .SetElementName("dataCadastro");

                map.MapMember(c => c.DataAlteracao)
                    .SetSerializer(new NullableSerializer<DateTime>(new DateTimeSerializer(DateTimeKind.Utc)))
                    .SetElementName("dataAlteracao");
            });
        }
    }
}
