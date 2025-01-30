using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;

namespace AutenticacaoDoisFatores.Servico.Excecoes
{
    public class ExcecoesCriacaoCliente(MensagensValidacaoCliente mensagem) : ApplicationException(mensagem.Descricao())
    {
        internal static void LinkConfirmacaoCadastroNaoInformado()
        {
            throw new ExcecoesCriacaoCliente(MensagensValidacaoCliente.LinkConfirmacaoCadastroNaoInformado);
        }
    }
}
