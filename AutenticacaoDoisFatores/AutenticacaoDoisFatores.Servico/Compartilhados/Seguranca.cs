using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Dominios;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutenticacaoDoisFatores.Servico.Compartilhados
{
    public static partial class Seguranca
    {
        public static byte[] ChaveDeAutenticacao()
        {
            var chaveDeAutenticacao = Environment.GetEnvironmentVariable("ADF_CHAVE_AUTENTICACAO");
            if (chaveDeAutenticacao is null || chaveDeAutenticacao.EstaVazio())
                throw new ApplicationException("A chave de autenticação não foi encontrada");

            var chaveEmBytes = Encoding.ASCII.GetBytes(chaveDeAutenticacao);
            return chaveEmBytes;
        }

        public static (string chave, string chaveCriptografada) GerarChaveDeAcessoComCriptografia()
        {
            var chave = Guid.NewGuid().ToString();

            var chaveCriptografada = Criptografia.CriptografarComSha512(chave);

            return (chave, chaveCriptografada);
        }
    }

    #region Geração de tokens

    public static partial class Seguranca
    {
        #region Perfis de segurança

        private static readonly string _perfilIdentificador = "identificadorEntidade";
        private static readonly string _perfilSeguranca = ClaimTypes.Role;

        #endregion

        #region Regras de segurança

        private static readonly string _confirmacaoDeCliente = "confirmacaoCliente";
        private static readonly string _geracaoNovaChaveCliente = "geracaoNovaChaveCliente";

        public static string RegraConfirmacaoDeCliente
        {
            get
            {
                return _confirmacaoDeCliente;
            }
        }

        public static string RegraGeracaoNovaChaveCliente
        {
            get
            {
                return _geracaoNovaChaveCliente;
            }
        }

        #endregion

        public static string GerarTokenDeConfirmacaoDeCliente(Guid idCliente)
        {
            return GerarToken([
                new(type: _perfilIdentificador, idCliente.ToString()),
                new(type: _perfilSeguranca, _confirmacaoDeCliente)
            ]);
        }

        public static string GerarTokenDeGeracaoNovaChaveDeAcesso(Guid idCliente)
        {
            return GerarToken([
                new(type: _perfilIdentificador, idCliente.ToString()),
                new(type: _perfilSeguranca, _geracaoNovaChaveCliente)
            ]);
        }

        private static string GerarToken(IEnumerable<Claim?> perfis)
        {
            var geradorDeToken = new JwtSecurityTokenHandler();

            var perfilDoToken = new ClaimsIdentity();
            perfilDoToken.AddClaims(perfis);

            var chave = ChaveDeAutenticacao();
            var chaveSimetrica = new SymmetricSecurityKey(chave);
            var credenciais = new SigningCredentials(chaveSimetrica, SecurityAlgorithms.HmacSha256Signature);

            var descritorDoToken = new SecurityTokenDescriptor()
            {
                Subject = perfilDoToken,
                SigningCredentials = credenciais,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddHours(2)
            };

            var token = geradorDeToken.CreateToken(descritorDoToken);
            return geradorDeToken.WriteToken(token);
        }
    }

    #endregion

    #region Leitura de tokens

    public static partial class Seguranca
    {
        public static Guid RetornarIdClienteTokenDeConfirmacaoDeCliente(string token)
        {
            var geradorDeToken = new JwtSecurityTokenHandler();
            var perfisDoToken = LerToken(token);
            var perfilDeIdentificador = perfisDoToken.FirstOrDefault(perfil => perfil.Type.Equals(_perfilIdentificador));
            if (perfilDeIdentificador is null)
                return Guid.Empty;

            var id = Guid.Parse(perfilDeIdentificador.Value);
            return id;
        }

        private static IEnumerable<Claim> LerToken(string token)
        {
            var geradorDeToken = new JwtSecurityTokenHandler();
            var tokenEmObjeto = geradorDeToken.ReadJwtToken(token);

            return tokenEmObjeto.Claims;
        }
    }

    #endregion
}
