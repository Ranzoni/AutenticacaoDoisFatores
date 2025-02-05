using System.ComponentModel;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens
{
    public enum MensagensValidacaoUsuario
    {
        [Description("O nome do usuário não é válido. Ele deve conter entre 3 e 50 caracteres.")]
        NomeInvalido,
        [Description("O nome de usuário não é válido. Ele deve conter entre 5 e 20 caracteres.")]
        NomeUsuarioInvalido,
        [Description("O e-mail do usuário não é válido.")]
        EmailInvalido,
        [Description("A senha do usuário não é válida. Ela deve conter letras maiúsculas e minúsculas, caracteres especiais e números.")]
        SenhaInvalida,
        [Description("O cliente referente ao usuário não foi informado.")]
        ClienteNaoInformado,
        [Description("Este nome de usuário já está cadastrado.")]
        NomeUsuarioJaCadastrado,
        [Description("Já existe um usuário com este endereço de e-mail.")]
        EmailJaCadastrado
    }
}
