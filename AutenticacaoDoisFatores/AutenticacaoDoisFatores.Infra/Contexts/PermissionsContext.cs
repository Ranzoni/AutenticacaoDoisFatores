using AutenticacaoDoisFatores.Infra.Configurations;
using AutenticacaoDoisFatores.Infra.Entities;
using AutenticacaoDoisFatores.Infra.Utils;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace AutenticacaoDoisFatores.Infra.Contexts
{
    public class PermissionsContext(string connectionString, string domainName)
    {
        private readonly MongoClient _mongoClient = new(connectionString);
        private readonly string _databaseName = domainName;
        private readonly string _permissionCollection = "permissao";
        private readonly string _auditCollection = "auditoria";

        internal IMongoCollection<Permission> Permissions => GetDatabase().GetCollection<Permission>(_permissionCollection);
        internal IMongoCollection<Audit> Audits => GetDatabase().GetCollection<Audit>(_auditCollection);

        public static void ApplyConfigurations()
        {
            PermissionConfiguration.Configure();
            AuditConfiguration.Configurar();
        }

        internal Audit? BuildAudit(Type? type, Guid entityId, AuditActions action, object detailss)
        {
            if (type == typeof(Permission))
                return new Audit(action, entityId, _permissionCollection, detailss);

            return null;
        }

        private IMongoDatabase GetDatabase()
        {
            return _mongoClient.GetDatabase(_databaseName);
        }
    }

    internal static class MongoExtensions
    {
        internal static async Task AddAsync<T>(this IMongoCollection<T> mongoCollection, T entity)
        {
            await mongoCollection.InsertOneAsync(entity);
        }

        internal static async Task UpdateAsync<TEntity>(this IMongoCollection<TEntity> mongoCollection, Expression<Func<TEntity, bool>> expressionFilter, IDictionary<Expression<Func<TEntity, object>>, object> fieldsToUpdate)
        {
            if (!fieldsToUpdate.Any())
                return;

            var filter = Builders<TEntity>.Filter.Where(expressionFilter);
            var update = Builders<TEntity>.Update;
            UpdateDefinition<TEntity>? action = null;

            foreach (var field in fieldsToUpdate)
                if (action is null)
                    action = update.Set(field.Key, field.Value);
                else
                    action = action.Set(field.Key, field.Value);

            await mongoCollection.UpdateManyAsync(filter, action);
        }

        internal static async Task<T> GetAsync<T>(this IMongoCollection<T> mongoCollection, Expression<Func<T, bool>> expressionFilter)
        {
            var result = await mongoCollection
                .Find(expressionFilter)
                .FirstOrDefaultAsync();

            return result;
        }
    }
}
