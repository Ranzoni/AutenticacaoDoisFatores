using System.ComponentModel;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens
{
    public enum MensagensValidacaoAppAutenticacao
    {
        [Description("A chave secreta para geração de QR Code não foi encontrada.")]
        ChaveSecretaNaoEncontrada,
        [Description("O código para autenticação não foi informado.")]
        CodigoNaoInformado
    }
}
