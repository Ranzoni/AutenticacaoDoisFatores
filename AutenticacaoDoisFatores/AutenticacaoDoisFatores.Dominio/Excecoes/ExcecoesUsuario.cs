using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;

namespace AutenticacaoDoisFatores.Dominio.Excecoes
{
    public class ExcecoesUsuario(MensagensValidacaoUsuario mensagem) : ApplicationException(mensagem.Descricao())
    {
        internal static void NomeInvalido()
        {
            throw new ExcecoesUsuario(MensagensValidacaoUsuario.NomeInvalido);
        }

        internal static void NomeUsuarioInvalido()
        {
            throw new ExcecoesUsuario(MensagensValidacaoUsuario.NomeUsuarioInvalido);
        }

        internal static void EmailInvalido()
        {
            throw new ExcecoesUsuario(MensagensValidacaoUsuario.EmailInvalido);
        }

        internal static void SenhaInvalida()
        {
            throw new ExcecoesUsuario(MensagensValidacaoUsuario.SenhaInvalida);
        }

        internal static void NomeUsuarioJaCadastrado()
        {
            throw new ExcecoesUsuario(MensagensValidacaoUsuario.NomeUsuarioJaCadastrado);
        }

        internal static void EmailJaCadastrado()
        {
            throw new ExcecoesUsuario(MensagensValidacaoUsuario.EmailJaCadastrado);
        }

        internal static void UsuarioNaoEncontrado()
        {
            throw new ExcecoesUsuario(MensagensValidacaoUsuario.UsuarioNaoEncontrado);
        }
    }
}
