using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Servico.Compartilhados;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;
using Mensageiro;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Usuarios.AutenticacoesDoisFatores
{
    public class EnviarCodAutenticacaoUsuarioPorEmail(EnvioDeEmail email, DominioDeCodDeAutenticacao dominio, INotificador notificador) : ITipoDeEnvioDeCodAutenticacaoUsuario
    {
        private readonly EnvioDeEmail _email = email;
        private readonly INotificador _notificador = notificador;
        private readonly DominioDeCodDeAutenticacao _dominio = dominio;

        public async Task<RespostaAutenticacaoDoisFatores?> EnviarAsync(Usuario usuario)
        {
            if (!EnvioEhValido(usuario))
                return null;

            var codigoAutenticacao = Seguranca.GerarCodigoAutenticacao();
            var codigoCriptografado = Criptografia.CriptografarComSha512(codigoAutenticacao);
            
            await _dominio.SalvarAsync(usuario.Id, codigoCriptografado);
            _email.EnviarCodigoAutenticacaoDoisFatores(usuario.Email, codigoAutenticacao);

            var token = Seguranca.GerarTokenCodAutenticacaoUsuario(usuario.Id);
            var resposta = new RespostaAutenticacaoDoisFatores(token);
            return resposta;
        }

        private bool EnvioEhValido(Usuario usuario)
        {
            if (!usuario.Ativo)
            {
                _notificador.AddMensagemNaoEncontrado(MensagensValidacaoUsuario.UsuarioNaoEncontrado);
                return false;
            }

            return true;
        }
    }
}
