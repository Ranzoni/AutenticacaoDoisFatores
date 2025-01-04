using System.ComponentModel;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens
{
    public enum MensagensCliente
    {
        [Description("O nome do cliente não é válido. Ele deve conter entre 3 e 50 caracteres.")]
        NomeInvalido,
        [Description("O e-mail do cliente não é válido.")]
        EmailInvalido,
        [Description("O nome de domínio do cliente não é válido. Ele não pode conter caracteres especiais, acentos, espaços ou pontuações.")]
        NomeDominioInvalido,
        [Description("O cliente não foi encontrado.")]
        ClienteNaoEncontrado,
        [Description("O domínio informado já está cadastrado.")]
        NomeDominioJaCadastrado,
        [Description("O e-mail informado já está cadastrado.")]
        EmailJaCadastrado
    }
}
