using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores.AutenticacoesDoisFatores;
using Microsoft.Extensions.DependencyInjection;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores
{
    public class AutenticadorUsuarioEmDoisFatores(IServiceProvider provedorDeServicos) : ITipoDeAutenticador
    {
        private readonly IServiceProvider _provedorDeServicos = provedorDeServicos;

        public async Task<object?> ExecutarAsync(Usuario usuario)
        {
            ITipoDeAutentidorUsuarioEmDoisFatores codAutenticacaoDoisFatores = usuario.TipoDeAutenticacao switch
            {
                TipoDeAutenticacao.Email => _provedorDeServicos.GetRequiredService<AutenticadorUsuarioEmDoisFatoresPorEmail>(),
                TipoDeAutenticacao.AppAutenticador => _provedorDeServicos.GetRequiredService<AutenticadorUsuarioEmDoisFatoresPorApp>(),
                _ => throw new ApplicationException($"O tipo de autenticação de usuário é inválido. Tipo: {usuario.TipoDeAutenticacao}.")
            };

            return await codAutenticacaoDoisFatores.EnviarAsync(usuario);
        }
    }
}
