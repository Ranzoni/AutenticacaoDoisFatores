using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;

namespace AutenticacaoDoisFatores.Dominio.Excecoes
{
    internal class ExcecoesCliente(MensagensCliente mensagem) : ApplicationException(mensagem.Descricao())
    {
        internal static void NomeNaoPreenchido()
        {
            throw new ExcecoesCliente(MensagensCliente.NomeNaoPreenchido);
        }

        internal static void EmailNaoPreenchido()
        {
            throw new ExcecoesCliente(MensagensCliente.EmailNaoPreenchido);
        }
    }
}
