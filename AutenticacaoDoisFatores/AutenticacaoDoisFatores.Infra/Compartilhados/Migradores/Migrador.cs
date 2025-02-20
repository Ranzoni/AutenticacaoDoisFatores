using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Infra.Contexto;

namespace AutenticacaoDoisFatores.Infra.Compartilhados.Migradores
{
    internal abstract class Migrador
    {
        private static readonly string _caminhoScriptTabelaMigracao = "Controle";

        protected static void AplicarMigracoes(string stringDeConexao, string pastaScripts)
        {
            var contexto = new ContextoCliente(stringDeConexao);

            var schemas = contexto.RetornarSchemas();
            foreach (var schema in schemas)
            {
                var arquivoTabelaMigracao = RetornarScriptsDiretorio(pastaScripts, _caminhoScriptTabelaMigracao).FirstOrDefault() ?? "";
                if (!arquivoTabelaMigracao.EstaVazio())
                {
                    var sql = File.ReadAllText(arquivoTabelaMigracao);
                    contexto.Executar(sql, schema);
                }

                var arquivosDeMigracao = RetornarScriptsDiretorio(pastaScripts);

                foreach (var arquivo in arquivosDeMigracao.OrderBy(f => Path.GetFileName(f)))
                {
                    var nomeArquivo = Path.GetFileName(arquivo);

                    if (nomeArquivo.EstaVazio() || contexto.ScriptMigrado(schema, nomeArquivo))
                        continue;

                    var sql = File.ReadAllText(arquivo);
                    contexto.Executar(sql, schema);

                    contexto.MarcarScriptComoMigrado(schema, nomeArquivo);
                }
            }
        }

        private static string[] RetornarScriptsDiretorio(params string[] caminhos)
        {
            var diretorio = Path.Combine(Directory.GetCurrentDirectory(), "Arquivos", "Scripts");
            foreach (var caminho in caminhos)
                diretorio = Path.Combine(diretorio, caminho);

            return Directory.GetFiles(diretorio, "*.sql");
        }
    }
}
