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
            var permissao = new Permissao(idUsuario, permissoes);

            await GetCollection().InsertOneAsync(permissao);
        }

        public async Task<IEnumerable<TipoDePermissao>> RetornarPermissoesAsync(Guid idUsuario)
        {
            var permissao = await GetCollection()
                .Find(p =>
                    p.IdUsuario.Equals(idUsuario))
                .FirstOrDefaultAsync();

            return permissao?.Permissoes ?? [];
        }

        public async Task EditarAsync(Guid idUsuario, IEnumerable<TipoDePermissao> permissoes)
        {
            var filtro = Builders<Permissao>.Filter.Eq(p => p.IdUsuario, idUsuario);
            var acao = Builders<Permissao>.Update.Set(pu => pu.Permissoes, permissoes);

            await GetCollection().UpdateOneAsync(filtro, acao);
        }
    }

    internal class Permissao
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; private set; } = "";
        [BsonElement("idUsuario")]
        [BsonRepresentation(BsonType.String)]
        public Guid IdUsuario { get; private set; }
        [BsonElement("tipoDePermissao")]
        public IEnumerable<TipoDePermissao> Permissoes { get; private set; } = [];
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; private set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? UpdatedAt { get; private set; }

        public Permissao() { }

        public Permissao(Guid idUsuario, IEnumerable<TipoDePermissao> permissoes)
        {
            Id = "";
            IdUsuario = idUsuario;
            Permissoes = permissoes;
            CreatedAt = DateTime.Now;
        }

        public Permissao(string id, Guid idUsuario, IEnumerable<TipoDePermissao> permissoes, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            IdUsuario = idUsuario;
            Permissoes = permissoes;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }
}
