using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Repositorios;

namespace AutenticacaoDoisFatores.Compartilhados
{
    internal class Intermediador(RequestDelegate proximo)
    {
        private readonly RequestDelegate _proximo = proximo;

        public async Task InvokeAsync(HttpContext contexto)
        {
            if (ContextoDeveSerIgnorado(contexto))
                await _proximo(contexto);

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

            contexto.Request.Headers.Append("Dominio", nomeDominio);

            return !nomeDominio.EstaVazio();
        }
    }
}
