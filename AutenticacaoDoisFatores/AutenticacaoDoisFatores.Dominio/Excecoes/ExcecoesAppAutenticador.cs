using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;

namespace AutenticacaoDoisFatores.Dominio.Excecoes
{
    public class ExcecoesAppAutenticador(MensagensValidacaoAppAutenticacao mensagem) : ApplicationException(mensagem.Descricao())
    {
        public static void ChaveSecretaNaoEncontrada()
        {
            throw new ExcecoesAppAutenticador(MensagensValidacaoAppAutenticacao.ChaveSecretaNaoEncontrada);
        }

        public static void CodigoNaoInformado()
        {
            throw new ExcecoesAppAutenticador(MensagensValidacaoAppAutenticacao.CodigoNaoInformado);
        }
    }
}
