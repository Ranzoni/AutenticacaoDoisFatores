using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;

namespace AutenticacaoDoisFatores.Dominio.Excecoes
{
    public class ExcecoesCodDeAutenticacao(MensagensValidacaoCodDeAutenticacao mensagem) : ApplicationException(mensagem.Descricao())
    {
        internal static void CodigoAutenticacaoVazio()
        {
            throw new ExcecoesCodDeAutenticacao(MensagensValidacaoCodDeAutenticacao.CodAutenticacaoVazio);
        }
    }
}
