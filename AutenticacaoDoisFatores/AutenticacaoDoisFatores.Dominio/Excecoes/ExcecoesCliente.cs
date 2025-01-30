using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;

namespace AutenticacaoDoisFatores.Dominio.Excecoes
{
    public class ExcecoesCliente(MensagensValidacaoCliente mensagem) : ApplicationException(mensagem.Descricao())
    {
        internal static void NomeInvalido()
        {
            throw new ExcecoesCliente(MensagensValidacaoCliente.NomeInvalido);
        }

        internal static void EmailInvalido()
        {
            throw new ExcecoesCliente(MensagensValidacaoCliente.EmailInvalido);
        }

        internal static void NomeDominioInvalido()
        {
            throw new ExcecoesCliente(MensagensValidacaoCliente.NomeDominioInvalido);
        }

        internal static void ChaveAcessoInvalida()
        {
            throw new ExcecoesCliente(MensagensValidacaoCliente.ChaveAcessoInvalida);
        }

        internal static void EmailJaCadastrado()
        {
            throw new ExcecoesCliente(MensagensValidacaoCliente.EmailJaCadastrado);
        }

        internal static void NomeDominioJaCadastrado()
        {
            throw new ExcecoesCliente(MensagensValidacaoCliente.NomeDominioJaCadastrado);
        }
    }
}
