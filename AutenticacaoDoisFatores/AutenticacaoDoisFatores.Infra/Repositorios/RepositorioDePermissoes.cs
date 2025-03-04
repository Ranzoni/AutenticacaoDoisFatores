using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Infra.Contexto;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace AutenticacaoDoisFatores.Infra.Repositorios
{
    public class RepositorioDePermissoes(ContextoPermissoes contexto) : IRepositorioDePermissoes
    {
        private readonly ContextoPermissoes _contexto = contexto;

        private IMongoCollection<Permissao> GetCollection()
        {
            return _contexto.GetDatabase().GetCollection<Permissao>("permissao");
        }

        public async Task AdicionarAsync(Guid idUsuario, IEnumerable<TipoDePermissao> permissoes)
        {
            var permissoesParaIncluir = permissoes
                .Select(tipoDePermissao => new Permissao(idUsuario, tipoDePermissao));

            await GetCollection().InsertManyAsync(permissoesParaIncluir);
        }

        public async Task<IEnumerable<TipoDePermissao>> RetornarPermissoes(Guid idUsuario)
        {
            return await GetCollection()
                .Find(p =>
                    p.IdUsuario.Equals(idUsuario))
                .Project(p => p.TipoDePermissao)
                .ToListAsync();
        }
    }

    public class Permissao
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; private set; } = "";
        [BsonElement("idUsuario")]
        [BsonRepresentation(BsonType.String)]
        public Guid IdUsuario { get; private set; }
        [BsonElement("tipoDePermissao")]
        public TipoDePermissao TipoDePermissao { get; private set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; private set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? UpdatedAt { get; private set; }

        public Permissao() { }

        public Permissao(Guid idUsuario, TipoDePermissao tipoDePermissao)
        {
            Id = "";
            IdUsuario = idUsuario;
            TipoDePermissao = tipoDePermissao;
            CreatedAt = DateTime.Now;
        }

        public Permissao(string id, Guid idUsuario, TipoDePermissao tipoDePermissao, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            IdUsuario = idUsuario;
            TipoDePermissao = tipoDePermissao;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }
}
