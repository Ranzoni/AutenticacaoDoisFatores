using StackExchange.Redis;

namespace AutenticacaoDoisFatores.Infra.Contexto
{
    public class ContextoDeCodigoDeAutenticacao
    {
        private readonly ConnectionMultiplexer _conexao;
        private readonly string _nomeDominio;

        public ContextoDeCodigoDeAutenticacao(string host, int porta, string usuario, string senha, string nomeDominio)
        {
            _conexao = ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    EndPoints = { { host, porta } },
                    User = usuario,
                    Password = senha
                }
            );

            _nomeDominio = nomeDominio;
        }

        public async Task SalvarAsync(string chave, string valor)
        {
            var db = _conexao.GetDatabase();
            await db.StringSetAsync(ChaveDominio(chave), valor, TimeSpan.FromMinutes(5));
        }

        public async Task<string> BuscarAsync(string chave)
        {
            var db = _conexao.GetDatabase();
            var valor = await db.StringGetAsync(ChaveDominio(chave));
            return valor.ToString();
        }

        private string ChaveDominio(string chave)
        {
            return $"{chave}_{_nomeDominio}";
        }
    }
}
