using AutenticacaoDoisFatores.Dominio.Compartilhados;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutenticacaoDoisFatores.Servico.Compartilhados
{
    public static class Seguranca
    {
        #region Perfis de segurnça

        private static readonly string _perfilIdentificador = "identificadorEntidade";
        private static readonly string _perfilSeguranca = ClaimTypes.Role;

        #endregion

        #region Regras de segurança

        private static readonly string _confirmacaoDeCliente = "confirmacaoCliente";

        public static string RegraConfirmacaoDeCliente
        {
            get
            {
                return _confirmacaoDeCliente;
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

        private static string GerarToken(IEnumerable<Claim?> perfis)
        {
            var geradorDeToken = new JwtSecurityTokenHandler();

            var perfilDoToken = new ClaimsIdentity();
            perfilDoToken.AddClaims(perfis);

            var chave = Chave();
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

        internal static Guid RetornarIdClienteTokenDeConfirmacaoDeCliente(string token)
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

        public static byte[] Chave()
        {
            var chaveDeAutenticacao = Environment.GetEnvironmentVariable("ADF_CHAVE_AUTENTICACAO");
            if (chaveDeAutenticacao is null || chaveDeAutenticacao.EstaVazio())
                throw new ApplicationException("A chave de autenticação não foi encontrada");

            var chaveEmBytes = Encoding.ASCII.GetBytes(chaveDeAutenticacao);
            return chaveEmBytes;
        }
    }
}
