using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Infra.Contexto;
using AutenticacaoDoisFatores.Infra.Repositorios;
using AutenticacaoDoisFatores.Servico.Compartilhados;

namespace AutenticacaoDoisFatores.Compartilhados
{
    internal class Intermediador(RequestDelegate proximo)
    {
        private readonly RequestDelegate _proximo = proximo;

        public async Task InvokeAsync(HttpContext contexto)
        {
            if (ContextoDeveSerIgnorado(contexto))
            {
                await _proximo(contexto);
                return;
            }

            if (!await ChaveApiEhValidaAsync(contexto))
            {
                contexto.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await _proximo(contexto);
        }

        private static bool ContextoDeveSerIgnorado(HttpContext contexto)
        {
            if (contexto.Request.Path.StartsWithSegments("/api/cliente"))
                return true;

            if (contexto.Request.Path.StartsWithSegments("/clientes"))
                return true;

            if (contexto.Request.Path.StartsWithSegments("/autenticacao"))
                return true;

            return false;
        }

        private static async Task<bool> ChaveApiEhValidaAsync(HttpContext contexto)
        {
            var chaveCliente = contexto.Request.Headers["Chave-API"].ToString();
            if (chaveCliente.EstaVazio())
                return false;

            var chaveCriptografada = Criptografia.CriptografarComSha512(chaveCliente);

            var repositorioClientes = contexto.RequestServices.GetRequiredService<IRepositorioDeClientes>();
            var nomeDominio = await repositorioClientes.RetornarNomeDominioAsync(chaveCriptografada) ?? "";

            if (nomeDominio.EstaVazio())
                return false;

            var token = contexto.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            if (!token.EstaVazio())
            {
                var stringDeConexao = Environment.GetEnvironmentVariable("ADF_CONEXAO_BANCO");
                if (stringDeConexao is null || stringDeConexao.EstaVazio())
                    throw new ApplicationException("A string de conexão com o banco de dados não foi encontrada");

                var contextoCliente = new ContextoCliente(stringDeConexao, nomeDominio);
                var repositorioUsuarios = new RepositorioDeUsuarios(contextoCliente);
                if (repositorioUsuarios is not null)
                {
                    var idUsuario = Seguranca.RetornarIdDoToken(token);
                    var usuario = await repositorioUsuarios.BuscarUsuarioPorDominioAsync(idUsuario, nomeDominio);
                    if (usuario is null || !usuario.Ativo)
                        return false;
                }
            }

            contexto.Request.Headers.Append("Dominio", nomeDominio);

            return !nomeDominio.EstaVazio();
        }
    }
}
