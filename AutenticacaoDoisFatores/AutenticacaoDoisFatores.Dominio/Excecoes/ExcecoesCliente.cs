using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;

namespace AutenticacaoDoisFatores.Dominio.Excecoes
{
    public class ExcecoesCliente(MensagensCliente mensagem) : ApplicationException(mensagem.Descricao())
    {
        internal static void NomeNaoPreenchido()
        {
            throw new ExcecoesCliente(MensagensCliente.NomeInvalido);
        }

        internal static void EmailNaoPreenchido()
        {
            throw new ExcecoesCliente(MensagensCliente.EmailInvalido);
        }
    }
}
