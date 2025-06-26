using System.ComponentModel;

namespace AutenticacaoDoisFatores.Domain.Shared.Messages
{
    public enum UserValidationMessages
    {
        [Description("O nome do usuário não é válido. Ele deve conter entre 3 e 50 caracteres.")]
        InvalidName,
        [Description("O nome de usuário não é válido. Ele deve conter entre 5 e 20 caracteres.")]
        InvalidUsername,
        [Description("O e-mail do usuário não é válido.")]
        InvalidEmail,
        [Description("A senha do usuário não é válida. Ela deve conter letras maiúsculas e minúsculas, caracteres especiais e números.")]
        InvalidPassword,
        [Description("O número de celular do usuário não é válido.")]
        InvalidPhone,
        [Description("Este nome de usuário já está cadastrado.")]
        UsernameAlreadyRegistered,
        [Description("Já existe um usuário com este endereço de e-mail.")]
        EmailAlreadyRegistered,
        [Description("O usuário não foi encontrado.")]
        UserNotFound,
        [Description("É necessário informar o nome de usuário ou o e-mail para realizar o acesso.")]
        UsernameOrEmailRequired,
        [Description("Nome de usuário, e-mail ou senha são inválidos.")]
        Unauthorized,
        [Description("A chave secreta do usuário é inválida.")]
        InvalidSecretKey,
        [Description("Este usuário já está ativo.")]
        UserAlreadyActivated
    }
}
