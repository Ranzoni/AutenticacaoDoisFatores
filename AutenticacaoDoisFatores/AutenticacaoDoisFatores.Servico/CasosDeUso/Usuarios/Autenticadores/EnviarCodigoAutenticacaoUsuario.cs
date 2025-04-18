using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.AutenticacoesDoisFatores;
using Microsoft.Extensions.DependencyInjection;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores
{
    public class EnviarCodigoAutenticacaoUsuario(IServiceProvider provedorDeServicos) : ITipoDeAutenticador
    {
        private readonly IServiceProvider _provedorDeServicos = provedorDeServicos;

        public async Task<object?> ExecutarAsync(Usuario usuario)
        {
            ITipoDeEnvioDeCodAutenticacaoUsuario codAutenticacaoDoisFatores = usuario.TipoDeAutenticacao switch
            {
                TipoDeAutenticacao.Email => _provedorDeServicos.GetRequiredService<EnviarCodAutenticacaoUsuarioPorEmail>(),
                TipoDeAutenticacao.SMS => _provedorDeServicos.GetRequiredService<EnviarCodAutenticacaoUsuarioPorSms>(),
                _ => throw new ApplicationException($"O tipo de autenticação de usuário é inválido. Tipo: {usuario.TipoDeAutenticacao}.")
            };

            return await codAutenticacaoDoisFatores.EnviarAsync(usuario);
        }
    }
}
