using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Infra.Contexto;

namespace AutenticacaoDoisFatores.Infra.Compartilhados.Migradores
{
    public abstract class Migrador(string stringDeConexao)
    {
        private readonly string _stringDeConexao = stringDeConexao;
        private static readonly string _caminhoScriptTabelaMigracao = "Controle";

        protected void AplicarMigracoes(string pastaScripts)
        {
            var contexto = new ContextoCliente(_stringDeConexao);

            var dominios = contexto.RetornarNomesDominios();
            foreach (var dominio in dominios)
            {
                var arquivoTabelaMigracao = RetornarScriptsDiretorio(pastaScripts, _caminhoScriptTabelaMigracao).FirstOrDefault() ?? "";
                if (!arquivoTabelaMigracao.EstaVazio())
                {
                    var sql = File.ReadAllText(arquivoTabelaMigracao);
                    contexto.Executar(sql, dominio);
                }

                var arquivosDeMigracao = RetornarScriptsDiretorio(pastaScripts);

                foreach (var arquivo in arquivosDeMigracao.OrderBy(f => Path.GetFileName(f)))
                {
                    var nomeArquivo = Path.GetFileName(arquivo);

                    if (nomeArquivo.EstaVazio() || contexto.ScriptMigrado(dominio, nomeArquivo))
                        continue;

                    var sql = File.ReadAllText(arquivo);
                    contexto.Executar(sql, dominio);

                    contexto.MarcarScriptComoMigrado(dominio, nomeArquivo);
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
