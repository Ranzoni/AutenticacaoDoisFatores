using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Users;
using AutenticacaoDoisFatores.Domain.Builders;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Entities;
using AutenticacaoDoisFatores.Domain.Filters;
using AutenticacaoDoisFatores.Domain.Repositories;
using AutenticacaoDoisFatores.Infra.Contexts;
using System.Data.Common;

namespace AutenticacaoDoisFatores.Infra.Repositories
{
    public class UserRepository(ClientContext context) : IUserRepository
    {
        private readonly ClientContext _context = context;

        #region Write

        public void Add(User entity)
        {
            var sql = $@"
                INSERT INTO {_context.DomainName}.""Users""
                    (""Id"", ""Name"", ""Username"", ""Email"", ""Password"", ""Phone"", ""SecretKey"", ""CreatedAt"", ""LastDataChange"")
                VALUES
                    ('{entity.Id}', '{entity.Name}', '{entity.Username}', '{entity.Email}', '{entity.Password}', {(entity.Phone is null ? "NULL" : entity.Phone)}, '{Encrypt.AesEncrypt(entity.SecretKey)}', '{entity.CreatedAt:yyyy-MM-dd HH:mm:ss}', '{entity.LastDataChange}');";

            _context.BuildCommand(
                entity: entity,
                commandType: CommandType.Inclusion,
                table: "Users",
                sql: sql);
        }

        public void Add(User entity, string domain)
        {
            var sql = $@"
                INSERT INTO {domain}.""Users""
                    (""Id"", ""Name"", ""Username"", ""Email"", ""Password"", ""CreatedAt"", ""Active"", ""IsAdmin"", ""SecretKey"", ""LastDataChange"")
                VALUES
                    ('{entity.Id}', '{entity.Name}', '{entity.Username}', '{entity.Email}', '{entity.Password}', '{entity.CreatedAt:yyyy-MM-dd HH:mm:ss}', {entity.Active}, {entity.IsAdmin}, '{Encrypt.AesEncrypt(entity.SecretKey)}', '{entity.LastDataChange}');";

            _context.BuildCommand(
                entity: entity,
                commandType: CommandType.Inclusion,
                table: "Users",
                sql: sql,
                domainName: domain);
        }

        public void Update(User entity)
        {
            var sql = $@"
                UPDATE {_context.DomainName}.""Users""
                SET
                    ""Name"" = '{entity.Name}',
                    ""Username"" = '{entity.Username}',
                    ""Email"" = '{entity.Email}',
                    ""Password"" = '{entity.Password}',
                    ""Phone"" = {(entity.Phone is null ? "NULL" : entity.Phone)},
                    ""Active"" = {entity.Active},
                    ""UpdatedAt"" = '{entity.UpdatedAt:yyyy-MM-dd HH:mm:ss}',
                    ""LastDataChange"" = '{entity.LastDataChange}'";

            if (entity.AuthType is null)
                sql += $@",""AuthType"" = NULL";
            else
                sql += $@",""AuthType"" = {(int)entity.AuthType}";

            if (entity.LastAccess is not null)
                sql += $@",""LastAccess"" = '{entity.LastAccess:yyyy-MM-dd HH:mm:ss}'";

            sql += $@" WHERE
                ""Id"" = '{entity.Id}';";

            _context.BuildCommand(
                entity: entity,
                commandType: CommandType.Change,
                table: "Users",
                sql: sql);
        }

        public void Remove(User entity)
        {
            var sql = $@"
                DELETE FROM
                    {_context.DomainName}.""Users""
                WHERE
                    ""Id"" = '{entity.Id}';";

            _context.BuildCommand(
                entity: entity,
                commandType: CommandType.Removal,
                table: "Users",
                sql: sql);
        }

        public async Task SaveChangesAsync()
        {
            await _context.ExecuteCommandsAsync();
        }

        #endregion

        #region Read

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var sql = $@"
                SELECT
                    {BuildUsersFields()}
                FROM
                    {_context.DomainName}.""Users"" u
                WHERE
                    u.""Id"" = '{id}'";

            return await _context.GetOneAsync(sql, BuildUser);
        }

        public async Task<bool> EmailExistsAsync(string email, Guid? id = null)
        {
            var sql = $@"
                SELECT
                    COUNT(1) > 0
                FROM
                    {_context.DomainName}.""Users"" u
                WHERE
                    u.""Email"" = '{email}'";

            if (id is not null)
                sql += $@" AND ""Id"" != '{id}'";

            return await _context.TrueResultAsync(sql);
        }

        public async Task<bool> UsernameExistsAsync(string username, Guid? id = null)
        {
            var sql = $@"
                SELECT
                    COUNT(1) > 0
                FROM
                    {_context.DomainName}.""Users"" u
                WHERE
                    u.""Username"" = '{username}'";

            if (id is not null)
                sql += $@" AND ""Id"" != '{id}'";

            return await _context.TrueResultAsync(sql);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var sql = $@"
                SELECT
                    {BuildUsersFields()}
                FROM
                    {_context.DomainName}.""Users"" u
                WHERE
                    u.""Username"" = '{username}'";

            return await _context.GetOneAsync(sql, BuildUser);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var sql = $@"
                SELECT
                    {BuildUsersFields()}
                FROM
                    {_context.DomainName}.""Users"" u
                WHERE
                    u.""Email"" = '{email}'";

            return await _context.GetOneAsync(sql, BuildUser);
        }

        public async Task<bool> UsernameExistsAsync(string username, string domainName)
        {
            var sql = $@"
                SELECT
                    COUNT(1) > 0
                FROM
                    {domainName}.""Users"" u
                WHERE
                    u.""Username"" = '{username}'";

            return await _context.TrueResultAsync(sql);
        }

        public async Task<bool> EmailExistsAsync(string email, string domainName)
        {
            var sql = $@"
                SELECT
                    COUNT(1) > 0
                FROM
                    {domainName}.""Users"" u
                WHERE
                    u.""Email"" = '{email}'";

            return await _context.TrueResultAsync(sql);
        }

        public async Task<bool> IsAdminAsync(Guid id)
        {
            var sql = $@"
                SELECT
                    COUNT(1) > 0
                FROM
                    {_context.DomainName}.""Users"" u
                WHERE
                    u.""Id"" = '{id}' AND
                    u.""IsAdmin"" = true";

            return await _context.TrueResultAsync(sql);
        }

        public async Task<User?> GetByIdAsync(Guid id, string domainName)
        {
            var sql = $@"
                SELECT
                    {BuildUsersFields()}
                FROM
                    {domainName}.""Users"" u
                WHERE
                    u.""Id"" = '{id}'";

            return await _context.GetOneAsync(sql, BuildUser);
        }

        public async Task<IEnumerable<User>> GetAllAsync(UserFilter filter)
        {
            var qtToSkip = (filter.Page - 1) * filter.Quantity;

            var sql = $@"
                SELECT
                    {BuildUsersFields()}
                FROM
                    {_context.DomainName}.""Users"" u";

            var query = "";

            if (!filter.Name!.IsNullOrEmptyOrWhiteSpaces())
                query = AddQueryCondition(query, $@"LOWER(u.""Name"") like '%{filter.Name!.ToLower()}%'");

            if (!filter.Username!.IsNullOrEmptyOrWhiteSpaces())
                query = AddQueryCondition(query, $@"LOWER(u.""Username"") like '%{filter.Username!.ToLower()}%'");

            if (filter.Active.HasValue)
                query = AddQueryCondition(query, $@"u.""Active"" = {filter.Active}");

            if (filter.LastAccessFrom.HasValue)
                query = AddQueryCondition(query, $@"u.""LastAccess""::DATE >= '%{filter.LastAccessFrom:yyyy-MM-dd}%'");

            if (filter.LastAccessUntil.HasValue)
                query = AddQueryCondition(query, $@"(u.""LastAccess"" IS NULL OR u.""LastAccess""::DATE <= '%{filter.LastAccessUntil:yyyy-MM-dd}%')");

            if (filter.CreatedFrom.HasValue)
                query = AddQueryCondition(query, $@"u.""DataCadastro""::DATE >= '{filter.CreatedFrom:yyyy-MM-dd}'");

            if (filter.CreatedUntil.HasValue)
                query = AddQueryCondition(query, $@"u.""DataCadastro""::DATE <= '{filter.CreatedUntil:yyyy-MM-dd}'");

            if (filter.UpdatedFrom.HasValue)
                query = AddQueryCondition(query, $@"u.""UpdatedAt""::DATE >= '{filter.UpdatedFrom:yyyy-MM-dd}'");

            if (filter.UpdatedUntil.HasValue)
                query = AddQueryCondition(query, $@"(u.""UpdatedAt"" IS NULL OR u.""UpdatedAt""::DATE <= '{filter.UpdatedUntil:yyyy-MM-dd}')");

            if (filter.IsAdmin.HasValue)
                query = AddQueryCondition(query, $@"u.""IsAdmin"" = {filter.IsAdmin}");

            sql += $@"{query}
                ORDER BY u.""Id""
                LIMIT {filter.Quantity}
                OFFSET {qtToSkip}";

            return await _context.GetManyAsync(sql,
                reader =>
                {
                    var users = new List<User>();

                    while (reader.Read())
                    {
                        var user = BuildUser(reader);
                        users.Add(user);
                    }

                    return users;
                });
        }

        private static string BuildUsersFields()
        {
            return @"u.""Id"",
                u.""Name"",
                u.""Username"",
                u.""Email"",
                u.""Password"",
                u.""Active"",
                u.""LastAccess"",
                u.""CreatedAt"",
                u.""UpdatedAt"",
                u.""IsAdmin"",
                u.""AuthType"",
                u.""Phone"",
                u.""SecretKey"",
                u.""LastDataChange""";
        }

        private static User BuildUser(DbDataReader reader)
        {
            return new UserBuilder()
                .WithId(reader.GetGuid(0))
                .WithName(reader.GetString(1))
                .WithUsername(reader.GetString(2))
                .WithEmail(reader.GetString(3))
                .WithPassword(reader.GetString(4))
                .WithActive(reader.GetBoolean(5))
                .WithLastAccess(reader.IsDBNull(6) ? null : reader.GetDateTime(6))
                .WithCreatedAt(reader.GetDateTime(7))
                .WithUpdatedAt(reader.IsDBNull(8) ? null : reader.GetDateTime(8))
                .WithIsAdminFlag(reader.GetBoolean(9))
                .WithAuthType(reader.IsDBNull(10) ? null : (AuthType)reader.GetInt16(10))
                .WithPhone(reader.IsDBNull(11) ? null : reader.GetInt64(11))
                .WithSecretKey(Encrypt.AesDecrypt(reader.GetString(12)))
                .WithLastDataChange(reader.GetString(13))
                .Build();
        }

        private static string AddQueryCondition(string sql, string condition)
        {
            if (sql.IsNullOrEmptyOrWhiteSpaces())
                sql += @"
                    WHERE ";
            else
                sql += @"
                    AND ";

            return sql + condition;
        }

        #endregion
    }
}
