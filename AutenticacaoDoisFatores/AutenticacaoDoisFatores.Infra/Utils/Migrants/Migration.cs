using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Infra.Contexts;

namespace AutenticacaoDoisFatores.Infra.Utils.Migrants
{
    public abstract class Migration(string connectionString)
    {
        private readonly string _connectionString = connectionString;
        private static readonly string _migrationScriptFolder = "Controle";

        protected async Task ExecuteScriptsAsync(string scriptsFolder)
        {
            var dominios = await ClientContext.GetAllDomainsNamesAsync(_connectionString);
            foreach (var dominio in dominios)
                await ExecuteScriptsAsync(dominio, scriptsFolder);
        }

        protected async Task ExecuteScriptsAsync(string domainName, string scriptsFolder)
        {
            var migrationTableFile = GetScriptsFilePath(scriptsFolder, _migrationScriptFolder).FirstOrDefault() ?? "";
            if (!migrationTableFile.IsNullOrEmptyOrWhiteSpaces())
            {
                var sql = File.ReadAllText(migrationTableFile);
                await ClientContext.ExecuteOnDomainAsync(sql, domainName, _connectionString);
            }

            var migrationFiles = GetScriptsFilePath(scriptsFolder);

            foreach (var file in migrationFiles.OrderBy(f => Path.GetFileName(f)))
            {
                var fileName = Path.GetFileName(file);

                if (fileName.IsNullOrEmptyOrWhiteSpaces() || await ClientContext.MigratedScriptAsync(domainName, fileName, _connectionString))
                    continue;

                var sql = File.ReadAllText(file);
                await ClientContext.ExecuteOnDomainAsync(sql, domainName, _connectionString);

                await ClientContext.SetScriptdAsMigratedAsync(domainName, fileName, _connectionString);
            }
        }

        private static string[] GetScriptsFilePath(params string[] paths)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Arquivos", "Scripts");
            foreach (var path in paths)
                folderPath = Path.Combine(folderPath, path);

            return Directory.GetFiles(folderPath, "*.sql");
        }
    }
}
