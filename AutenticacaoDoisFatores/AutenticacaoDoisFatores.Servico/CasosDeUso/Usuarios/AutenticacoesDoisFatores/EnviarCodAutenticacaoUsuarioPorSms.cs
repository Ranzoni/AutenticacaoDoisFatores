using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.AutenticacoesDoisFatores
{
    public class EnviarCodAutenticacaoUsuarioPorSms : ITipoDeEnvioDeCodAutenticacaoUsuario
    {
        public Task<RespostaAutenticacaoDoisFatores?> EnviarAsync(Usuario usuario)
        {
            throw new NotImplementedException();
        }
    }
}
