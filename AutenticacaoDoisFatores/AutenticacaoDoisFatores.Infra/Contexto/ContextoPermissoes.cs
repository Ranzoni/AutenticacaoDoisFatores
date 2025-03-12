using AutenticacaoDoisFatores.Infra.Configuracoes;
using AutenticacaoDoisFatores.Infra.Entidades;
using AutenticacaoDoisFatores.Infra.Utilitarios;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace AutenticacaoDoisFatores.Infra.Contexto
{
    public class ContextoPermissoes(string stringDeConexao, string nomeDominio)
    {
        private readonly MongoClient _mongoClient = new(stringDeConexao);
        private readonly string _nomeBanco = nomeDominio;
        private readonly string _nomeColecaoPermissao = "permissao";

        internal IMongoCollection<Permissao> Permissoes => GetDatabase().GetCollection<Permissao>(_nomeColecaoPermissao);
        internal IMongoCollection<Auditoria> Audiorias => GetDatabase().GetCollection<Auditoria>("auditoria");

        public static void AplicarConfiguracoes()
        {
            PermissaoConfiguracao.Configurar();
            AuditoriaConfiguracao.Configurar();
        }

        internal Auditoria? MontarAuditoria(Type? tipo, Guid idEntidade, AcoesDeAuditoria acao, object detalhes)
        {
            if (tipo == typeof(Permissao))
                return new Auditoria(acao, idEntidade, _nomeColecaoPermissao, detalhes);

            return null;
        }

        private IMongoDatabase GetDatabase()
        {
            return _mongoClient.GetDatabase(_nomeBanco);
        }
    }

    internal static class ExtensoesMongo
    {
        internal static async Task AdicionarAsync<T>(this IMongoCollection<T> mongoCollection, T entidade)
        {
            await mongoCollection.InsertOneAsync(entidade);
        }

        internal static async Task EditarAsync<TEntidade, TCampo>(this IMongoCollection<TEntidade> mongoCollection, Expression<Func<TEntidade, bool>> filtroExpressao, Expression<Func<TEntidade, TCampo>> campo, TCampo valor)
        {
            var filtro = Builders<TEntidade>.Filter.Where(filtroExpressao);
            var acao = Builders<TEntidade>.Update.Set(campo, valor);

            await mongoCollection.UpdateManyAsync(filtro, acao);
        }

        internal static async Task<T> BuscarUnicoAsync<T>(this IMongoCollection<T> mongoCollection, Expression<Func<T, bool>> filtroExpressao)
        {
            var resultado = await mongoCollection
                .Find(filtroExpressao)
                .FirstOrDefaultAsync();

            return resultado;
        }
    }
}
