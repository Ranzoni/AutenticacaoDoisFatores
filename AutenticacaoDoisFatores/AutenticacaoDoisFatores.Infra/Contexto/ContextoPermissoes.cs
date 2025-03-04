using MongoDB.Driver;

namespace AutenticacaoDoisFatores.Infra.Contexto
{
    public class ContextoPermissoes(string stringDeConexao, string nomeDominio)
    {
        private readonly MongoClient _cliente = new(stringDeConexao);
        private readonly string _nomeBanco = nomeDominio;

        public IMongoDatabase GetDatabase()
        {
            return _cliente.GetDatabase(_nomeBanco);
        }
    }
}
