using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Entities;
using Newtonsoft.Json;
using Npgsql;
using System.Data.Common;

namespace AutenticacaoDoisFatores.Infra.Contexts
{
    public enum CommandType
    {
        Inclusion,
        Change,
        Removal
    }

    public partial class ClientContext(string connectionString, string domainName)
    {
        private readonly string _connectionString = connectionString;
        private readonly Queue<string> _comands = [];

        public string DomainName { get; private set; } = domainName;

        public void BuildCommand(AuditedEntity entity, CommandType commandType, string table, string sql, string? domainName = null)
        {
            _comands.Enqueue(sql);

            var json = JsonConvert.SerializeObject(entity);

            var action = "";
            switch (commandType)
            {
                case CommandType.Inclusion:
                    action = nameof(CommandType.Inclusion);
                    break;
                case CommandType.Change:
                    action = nameof(CommandType.Change);
                    json = JsonConvert.SerializeObject(entity.GetUpdates());
                    break;
                case CommandType.Removal:
                    action = nameof(CommandType.Removal);
                    break;
            }

            var domain = domainName is null || domainName.IsNullOrEmptyOrWhiteSpaces() ? DomainName : domainName;

            _comands.Enqueue($@"
                INSERT INTO {domain}.""Audits""
                    (""Id"", ""Action"", ""EntityId"", ""Table"", ""Details"", ""Date"")
                VALUES
                    ('{Guid.NewGuid()}', '{action}', '{entity.Id}', '{table}', '{json}', '{DateTime.Now:yyyy-MM-dd HH:mm:ss}');");
        }

        public async Task ExecuteCommandsAsync()
        {
            var sql = "BEGIN;";

            foreach (var command in _comands)
                sql += $"{command}\n";

            sql += "COMMIT;";

            await ExecuteOnDomainAsync(sql, DomainName, _connectionString);
        }

        public async Task<T?> GetOneAsync<T>(string sql, Func<DbDataReader, T> action)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            if (reader is null)
                return default;

            if (await reader.ReadAsync())
                return action(reader);

            return default;
        }

        public async Task<IEnumerable<T>> GetManyAsync<T>(string sql, Func<DbDataReader, IEnumerable<T>> action)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            if (reader is null)
                return [];

            return action(reader);
        }

        public async Task<bool> TrueResultAsync(string sql)
        {
            using var conexao = new NpgsqlConnection(_connectionString);
            conexao.Open();

            using var comando = new NpgsqlCommand(sql, conexao);
            var resultado = await comando.ExecuteScalarAsync();

            return resultado != null && (bool)resultado;
        }
    }

    #region Migration Logic

    public partial class ClientContext
    {
        private static readonly string _tableName = "__AdfMigrations";

        public static async Task<IEnumerable<string>> GetAllDomainsNamesAsync(string connectionString)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = @"
                SELECT
                    schema_name 
                FROM
                    information_schema.schemata 
                WHERE
                    schema_name NOT LIKE 'pg_%' 
                    AND schema_name != 'information_schema'
                    AND schema_name != 'public';";

            using var command = new NpgsqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            if (reader is null)
                throw new NpgsqlException("Não foi possível fazer a leitura do comando no PostgreSQL");

            var domains = new List<string>();
            while (await reader.ReadAsync())
            {
                var domain = reader["schema_name"]?.ToString() ?? "";
                if (domain.IsNullOrEmptyOrWhiteSpaces())
                    continue;

                domains.Add(domain);
            }

            return domains;
        }

        public static async Task ExecuteOnDomainAsync(string sql, string domainName, string connectionString)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            if (!domainName.IsNullOrEmptyOrWhiteSpaces())
            {
                var setSchema = $"SET search_path TO {domainName};";
                sql = setSchema + sql;
            }

            using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteScalarAsync();
        }

        public static async Task<bool> MigratedScriptAsync(string domainName, string fileName, string connectionString)
        {
            using var conexao = new NpgsqlConnection(connectionString);
            conexao.Open();

            var sql = $@"
                SELECT
                    COUNT(1) > 0
                FROM
                    {domainName}.""{_tableName}"" m
                WHERE
                    LOWER(m.""FileName"") = '{fileName.ToLower()}'";

            using var comando = new NpgsqlCommand(sql, conexao);
            var resultado = await comando.ExecuteScalarAsync();

            return resultado != null && (bool)resultado;
        }

        public static async Task SetScriptdAsMigratedAsync(string domainName, string fileName, string connectionString)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var sql = $@"
                INSERT INTO {domainName}.""{_tableName}""
                    (""FileName"", ""ExecutedAt"")
                VALUES
                    ('{fileName}', NOW())";

            using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteScalarAsync();
        }
    }

    #endregion
}
