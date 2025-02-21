using AutenticacaoDoisFatores.Dominio.Compartilhados;
using Npgsql;

namespace AutenticacaoDoisFatores.Infra.Contexto
{
    public class ContextoCliente(string stringDeConexao)
    {
        private readonly string _stringDeConexao = stringDeConexao;

        public IEnumerable<string> RetornarNomesDominios()
        {
            using var conexao = new NpgsqlConnection(_stringDeConexao);
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
            while (leitor.Read())
            {
                var dominio = leitor["schema_name"]?.ToString() ?? "";
                if (dominio.EstaVazio())
                    continue;

                listaDominios.Add(dominio);
            }

            return listaDominios;
        }

        public void Executar(string sql, string schema)
        {
            using var conexao = new NpgsqlConnection(_stringDeConexao);
            conexao.Open();

            var alterarSchema = $"SET search_path TO {schema};";
            sql = alterarSchema + sql;

            using var comando = new NpgsqlCommand(sql, conexao);
            comando.ExecuteScalar();
        }

        public bool ScriptMigrado(string nomeDominio, string nomeArquivo)
        {
            using var conexao = new NpgsqlConnection(_stringDeConexao);
            conexao.Open();

            var sql = $@"
                SELECT
                    COUNT(1) > 0
                FROM
                    {nomeDominio}.""__MigracoesAdf"" m
                WHERE
                    LOWER(m.""NomeArquivo"") = '{nomeArquivo.ToLower()}'";

            using var comando = new NpgsqlCommand(sql, conexao);
            var resultado = comando.ExecuteScalar();

            return resultado != null && (bool)resultado;
        }

        public void MarcarScriptComoMigrado(string nomeDominio, string nomeArquivo)
        {
            using var conexao = new NpgsqlConnection(_stringDeConexao);
            conexao.Open();

            var sql = $@"
                INSERT INTO {nomeDominio}.""__MigracoesAdf""
                    (""NomeArquivo"", ""DataExecucao"")
                VALUES
                    ('{nomeArquivo}', NOW())";

            using var comando = new NpgsqlCommand(sql, conexao);
            comando.ExecuteScalar();
        }
    }
}
