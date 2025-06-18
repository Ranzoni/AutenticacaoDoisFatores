using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Domains;
using AutenticacaoDoisFatores.Domain.Repositories;

namespace AutenticacaoDoisFatores.Shared
{
    internal class Intermediator(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (ContextShouldBeIgnored(context))
            {
                await _next(context);
                return;
            }

            if (!await GetUserDomain(context))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await _next(context);
        }

        private static bool ContextShouldBeIgnored(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/cliente"))
                return true;

            if (context.Request.Path.StartsWithSegments("/clientes"))
                return true;

            if (context.Request.Path.StartsWithSegments("/autenticacao"))
                return true;

            return false;
        }

        private static async Task<bool> GetUserDomain(HttpContext context)
        {
            var clientKey = context.Request.Headers["Chave-API"].ToString();
            if (clientKey.IsNullOrEmptyOrWhiteSpaces())
                return false;

            var encryptedKey = Encrypt.EncryptWithSha512(clientKey);

            var clientRepository = context.RequestServices.GetRequiredService<IClientRepository>();
            var domainName = await clientRepository.GetDomainNameByAccessKeyAsync(encryptedKey) ?? "";

            if (domainName.IsNullOrEmptyOrWhiteSpaces())
                return false;

            context.Request.Headers.Append("Dominio", domainName);

            return true;
        }
    }
}
