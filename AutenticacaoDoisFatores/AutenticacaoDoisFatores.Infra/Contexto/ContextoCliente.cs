using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades;
using Newtonsoft.Json;
using Npgsql;
using System.Data.Common;

namespace AutenticacaoDoisFatores.Infra.Contexto
{
    public enum TipoComando
    {
        Inclusao,
        Alteracao,
        Exclusao
    }

    public partial class ContextoCliente(string stringDeConexao, string nomeDominio)
    {
        private readonly string _stringDeConexao = stringDeConexao;
        private readonly Queue<string> _comandos = [];

        public string NomeDominio { get; private set; } = nomeDominio;

        public void PrepararComando(EntidadeComAuditoria entidade, TipoComando tipo, string tabela, string sql)
        {
            _comandos.Enqueue(sql);

            var json = JsonConvert.SerializeObject(entidade);

            var acao = "";
            switch (tipo)
            {
                case TipoComando.Inclusao:
                    acao = "Inclusão";
                    break;
                case TipoComando.Alteracao:
                    acao = "Modificação";
                    json = JsonConvert.SerializeObject(entidade.RetornarAlteracoes());
                    break;
                case TipoComando.Exclusao:
                    acao = "Exclusão";
                    break;
            }

            _comandos.Enqueue($@"
                INSERT INTO {NomeDominio}.""Auditorias""
                    (""Id"", ""Acao"", ""IdEntidade"", ""Tabela"", ""Detalhes"", ""Data"")
                VALUES
                    ('{Guid.NewGuid()}', '{acao}', '{entidade.Id}', '{tabela}', '{json}', '{DateTime.Now:yyyy-MM-dd HH:mm:ss}');");
        }

        public async Task ExecutarComandosAsync()
        {
            var sql = "BEGIN;";

            foreach (var comando in _comandos)
                sql += $"{comando}\n";

            sql += "COMMIT;";

            await ExecutarEmDominioAsync(sql, NomeDominio, _stringDeConexao);
        }

        public async Task<T?> LerUnicoAsync<T>(string sql, Func<DbDataReader, T> acao)
        {
            using var conexao = new NpgsqlConnection(_stringDeConexao);
            await conexao.OpenAsync();

            using var comando = new NpgsqlCommand(sql, conexao);
            using var leitor = await comando.ExecuteReaderAsync();

            if (leitor is null)
                return default;

            if (await leitor.ReadAsync())
                return acao(leitor);

            return default;
        }

        public async Task<bool> ConsultaEhVerdadeiraAsync(string sql)
        {
            using var conexao = new NpgsqlConnection(_stringDeConexao);
            conexao.Open();

            using var comando = new NpgsqlCommand(sql, conexao);
            var resultado = await comando.ExecuteScalarAsync();

            return resultado != null && (bool)resultado;
        }
    }

    #region Lógica de Migração

    public partial class ContextoCliente
    {
        public static async Task<IEnumerable<string>> RetornarNomesDominiosAsync(string stringDeConexao)
        {
            using var conexao = new NpgsqlConnection(stringDeConexao);
            conexao.Open();

            var sql = @"
                SELECT
                    schema_name 
                FROM
                    information_schema.schemata 
                WHERE
                    schema_name NOT LIKE 'pg_%' 
                    AND schema_name != 'information_schema'
                    AND schema_name != 'public';";

            using var comando = new NpgsqlCommand(sql, conexao);
            using var leitor = comando.ExecuteReader();
            if (leitor is null)
                throw new NpgsqlException("Não foi possível fazer a leitura do comando no PostgreSQL");

            var listaDominios = new List<string>();
            while (await leitor.ReadAsync())
            {
                var dominio = leitor["schema_name"]?.ToString() ?? "";
                if (dominio.EstaVazio())
                    continue;

                listaDominios.Add(dominio);
            }

            return listaDominios;
        }

        public static async Task ExecutarEmDominioAsync(string sql, string dominio, string stringDeConexao)
        {
            using var conexao = new NpgsqlConnection(stringDeConexao);
            conexao.Open();

            var alterarSchema = $"SET search_path TO {dominio};";
            sql = alterarSchema + sql;

            using var comando = new NpgsqlCommand(sql, conexao);
            await comando.ExecuteScalarAsync();
        }

        public static async Task<bool> ScriptMigradoAsync(string nomeDominio, string nomeArquivo, string stringDeConexao)
        {
            using var conexao = new NpgsqlConnection(stringDeConexao);
            conexao.Open();

            var sql = $@"
                SELECT
                    COUNT(1) > 0
                FROM
                    {nomeDominio}.""__MigracoesAdf"" m
                WHERE
                    LOWER(m.""NomeArquivo"") = '{nomeArquivo.ToLower()}'";

            using var comando = new NpgsqlCommand(sql, conexao);
            var resultado = await comando.ExecuteScalarAsync();

            return resultado != null && (bool)resultado;
        }

        public static async Task MarcarScriptComoMigradoAsync(string nomeDominio, string nomeArquivo, string stringDeConexao)
        {
            using var conexao = new NpgsqlConnection(stringDeConexao);
            conexao.Open();

            var sql = $@"
                INSERT INTO {nomeDominio}.""__MigracoesAdf""
                    (""NomeArquivo"", ""DataExecucao"")
                VALUES
                    ('{nomeArquivo}', NOW())";

            using var comando = new NpgsqlCommand(sql, conexao);
            await comando.ExecuteScalarAsync();
        }
    }

    #endregion
}
