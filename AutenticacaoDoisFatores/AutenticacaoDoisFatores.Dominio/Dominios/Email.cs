using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Servicos;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public class Email(IServicoDeEmail servico)
    {
        private readonly IServicoDeEmail _servico = servico;

        public void EnviarConfirmacaoDeCadastroDeCliente(string para)
        {
            if (para.EstaVazio() || !para.EhEmail())
                ExcecoesEmail.EmailDestinoInvalido();

            var tituloConfirmacaoCadastro = MensagensEnvioEmail.TituloConfirmacaoCadastroCliente.Descricao() ?? "";
            var mensagemConfirmacaoCadastro = MensagensEnvioEmail.MsgConfirmacaoCadastroCliente.Descricao() ?? "";

            _servico.Enviar(para: para, titulo: tituloConfirmacaoCadastro, mensagem: mensagemConfirmacaoCadastro);
        }
    }
}
