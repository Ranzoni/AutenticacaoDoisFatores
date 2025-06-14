using StackExchange.Redis;

namespace AutenticacaoDoisFatores.Infra.Contexts
{
    public class AuthCodeContext
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly string _domainName;

        public AuthCodeContext(string host, int port, string user, string password, string domainName)
        {
            _connection = ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    EndPoints = { { host, port } },
                    User = user,
                    Password = password
                }
            );

            _domainName = domainName;
        }

        public async Task SaveAsync(string key, string value)
        {
            var db = _connection.GetDatabase();
            await db.StringSetAsync(DomainKey(key), value, TimeSpan.FromMinutes(5));
        }

        public async Task<string> GetByKeyAsync(string key)
        {
            var db = _connection.GetDatabase();
            var value = await db.StringGetAsync(DomainKey(key));
            return value.ToString();
        }

        private string DomainKey(string key)
        {
            return $"{key}_{_domainName}";
        }
    }
}
