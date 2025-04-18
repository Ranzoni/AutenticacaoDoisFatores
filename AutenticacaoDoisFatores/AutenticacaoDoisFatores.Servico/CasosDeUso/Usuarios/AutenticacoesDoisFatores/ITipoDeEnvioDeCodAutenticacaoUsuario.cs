using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.AutenticacoesDoisFatores
{
    internal interface ITipoDeEnvioDeCodAutenticacaoUsuario
    {
        Task<RespostaAutenticacaoDoisFatores?> EnviarAsync(Usuario usuario);
    }
}
