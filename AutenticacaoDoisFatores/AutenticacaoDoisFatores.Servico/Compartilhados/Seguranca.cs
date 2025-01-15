using AutenticacaoDoisFatores.Dominio.Compartilhados;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutenticacaoDoisFatores.Servico.Compartilhados
{
    public static class Seguranca
    {
        private static readonly string _perfilSeguranca = "acaoToken";
        private static readonly string _confirmacaoDeCliente = "confirmacaoCliente";

        internal static string GerarTokenDeConfirmacaoDeCliente(string email)
        {
            return GerarToken([
                new(type: ClaimTypes.Email, email),
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
