using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.Autenticadores.AutenticacoesDoisFatores
{
    internal interface ITipoDeAutentidorUsuarioEmDoisFatores
    {
        Task<RespostaAutenticacaoDoisFatores?> EnviarAsync(Usuario usuario);
    }
}
