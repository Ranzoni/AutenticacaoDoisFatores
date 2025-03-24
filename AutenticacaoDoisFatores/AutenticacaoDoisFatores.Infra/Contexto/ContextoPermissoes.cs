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
        private readonly string _nomeColecaoAuditoria = "auditoria";

        internal IMongoCollection<Permissao> Permissoes => GetDatabase().GetCollection<Permissao>(_nomeColecaoPermissao);
        internal IMongoCollection<Auditoria> Audiorias => GetDatabase().GetCollection<Auditoria>(_nomeColecaoAuditoria);

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

        internal static async Task EditarAsync<TEntidade>(this IMongoCollection<TEntidade> mongoCollection, Expression<Func<TEntidade, bool>> filtroExpressao, IDictionary<Expression<Func<TEntidade, object>>, object> camposParaEditar)
        {
            if (!camposParaEditar.Any())
                return;

            var filtro = Builders<TEntidade>.Filter.Where(filtroExpressao);
            var update = Builders<TEntidade>.Update;
            UpdateDefinition<TEntidade>? acao = null;

            foreach (var campo in camposParaEditar)
                if (acao is null)
                    acao = update.Set(campo.Key, campo.Value);
                else
                    acao = acao.Set(campo.Key, campo.Value);

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
