using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
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

        public static bool ComposicaoSenhaEhValida(string senha)
        {
            return !senha.EstaVazio() && senha.ExistemLetrasMaiusculas() && senha.ExistemLetrasMinusculas() && senha.ExistemNumeros() && senha.ExistemCaracteresEspeciaisAcentosOuPontuacoes() && senha.Length >= 7 && senha.Length <= 50;
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
        private static readonly string _usuarioAutenticado = "usuarioAutenticado";
        private static readonly string _criacaoDeUsuario = "criacaoDeUsuario";
        private static readonly string _ativarUsuario = "ativacaoDeUsuario";
        private static readonly string _desativarUsuario = "desativacaoDeUsuario";
        private static readonly string _trocarSenhaUsuario = "trocarSenhaDeUsuario";
        private static readonly string _definirPermissoes = "definirPermissoes";
        private static readonly string _exclusaoDeUsuario = "exclusaoDeUsuario";

        private static readonly Dictionary<TipoDePermissao, string> _perfisPermissoes = new()
        {
            { TipoDePermissao.CriarUsuario, _criacaoDeUsuario },
            { TipoDePermissao.AtivarUsuario, _ativarUsuario },
            { TipoDePermissao.DesativarUsuario, _desativarUsuario },
            { TipoDePermissao.TrocarSenhaUsuario, _trocarSenhaUsuario },
            { TipoDePermissao.DefinirPermissoes, _definirPermissoes },
            { TipoDePermissao.ExcluirUsuario, _exclusaoDeUsuario },
        };

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

        public static string RegraCriacaoDeUsuario
        {
            get
            {
                return _criacaoDeUsuario;
            }
        }

        public static string RegraAtivacaoUsuario
        {
            get
            {
                return _ativarUsuario;
            }
        }

        public static string RegraDesativacaoUsuario
        {
            get
            {
                return _desativarUsuario;
            }
        }

        public static string RegraTrocarSenhaUsuario
        {
            get
            {
                return _trocarSenhaUsuario;
            }
        }

        public static string RegraDefinirPermissoes
        {
            get
            {
                return _definirPermissoes;
            }
        }

        public static string RegraExclusaoDeUsuario
        {
            get
            {
                return _exclusaoDeUsuario;
            }
        }

        #endregion

        public static string GerarTokenAutenticacaoUsuario(Guid idUsuario, IEnumerable<TipoDePermissao>? permissoes)
        {
            var perfis = new List<Claim>()
            {
                new(type: _perfilIdentificador, idUsuario.ToString()),
                new(type: _perfilSeguranca, _usuarioAutenticado)
            };

            foreach (var permissao in permissoes ?? [])
            {
                var perfilPermissao = _perfisPermissoes[permissao];
                perfis.Add(new(type: _perfilSeguranca, perfilPermissao));
            }

            return GerarToken(perfis);
        }

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
        public static Guid RetornarIdDoToken(string token)
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
