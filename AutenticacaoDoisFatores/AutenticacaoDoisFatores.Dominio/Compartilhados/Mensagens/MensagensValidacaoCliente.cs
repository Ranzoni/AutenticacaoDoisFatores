using System.ComponentModel;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens
{
    public enum MensagensValidacaoCliente
    {
        [Description("O nome do cliente não é válido. Ele deve conter entre 3 e 50 caracteres.")]
        NomeInvalido,
        [Description("O e-mail do cliente não é válido.")]
        EmailInvalido,
        [Description("O nome de domínio do cliente não é válido. Ele não pode conter caracteres especiais, acentos, espaços ou pontuações.")]
        NomeDominioInvalido,
        [Description("A chave de acesso gerada está inválida.")]
        ChaveAcessoInvalida,
        [Description("O cliente não foi encontrado.")]
        ClienteNaoEncontrado,
        [Description("O domínio informado já está cadastrado.")]
        NomeDominioJaCadastrado,
        [Description("O e-mail informado já está cadastrado.")]
        EmailJaCadastrado,
        [Description("O link para a confirmação do cadastro não foi informado.")]
        LinkConfirmacaoCadastroNaoInformado,
        [Description("O token desta requisição é inválido.")]
        TokenInvalido,
        [Description("Este cliente já foi ativado.")]
        ClienteJaAtivado,
        [Description("Este cliente não está ativo!")]
        ClienteNaoAtivo
    }
}
