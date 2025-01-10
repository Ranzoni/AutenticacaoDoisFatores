using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;

namespace AutenticacaoDoisFatores.Dominio.Excecoes
{
    public class ExcecoesEmail(MensagensValidacaoEmail mensagem) : ApplicationException(mensagem.Descricao())
    {
        internal static void EmailDestinoInvalido()
        {
            throw new ExcecoesEmail(MensagensValidacaoEmail.EmailDestinoNaoInvalido);
        }

        internal static void TituloEmailNaoInformado()
        {
            throw new ExcecoesEmail(MensagensValidacaoEmail.TituloEmailNaoInformado);
        }

        internal static void MensagemEmailNaoInformada()
        {
            throw new ExcecoesEmail(MensagensValidacaoEmail.MensagemEmailNaoInformada);
        }

        public static void EnderecoEmailEnvioNaoConfigurado()
        {
            throw new ExcecoesEmail(MensagensValidacaoEmail.EnderecoEmailEnvioNaoConfigurado);
        }
    }
}
