using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Infra.Contexto;

namespace AutenticacaoDoisFatores.Infra.Compartilhados.Migradores
{
    public abstract class Migrador(string stringDeConexao)
    {
        private readonly string _stringDeConexao = stringDeConexao;
        private static readonly string _caminhoScriptTabelaMigracao = "Controle";

        protected async Task ExecutarScriptsAsync(string pastaScripts)
        {
            var contexto = new ContextoCliente(_stringDeConexao);

            var dominios = await contexto.RetornarNomesDominiosAsync();
            foreach (var dominio in dominios)
                await ExecutarScriptsAsync(dominio, pastaScripts);
        }

        protected async Task ExecutarScriptsAsync(string dominio, string pastaScripts)
        {
            var contexto = new ContextoCliente(_stringDeConexao);

            var arquivoTabelaMigracao = RetornarScriptsDiretorio(pastaScripts, _caminhoScriptTabelaMigracao).FirstOrDefault() ?? "";
            if (!arquivoTabelaMigracao.EstaVazio())
            {
                var sql = File.ReadAllText(arquivoTabelaMigracao);
                await contexto.ExecutarAsync(sql, dominio);
            }

            var arquivosDeMigracao = RetornarScriptsDiretorio(pastaScripts);

            foreach (var arquivo in arquivosDeMigracao.OrderBy(f => Path.GetFileName(f)))
            {
                var nomeArquivo = Path.GetFileName(arquivo);

                if (nomeArquivo.EstaVazio() || await contexto.ScriptMigradoAsync(dominio, nomeArquivo))
                    continue;

                var sql = File.ReadAllText(arquivo);
                await contexto.ExecutarAsync(sql, dominio);

                await contexto.MarcarScriptComoMigradoAsync(dominio, nomeArquivo);
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
