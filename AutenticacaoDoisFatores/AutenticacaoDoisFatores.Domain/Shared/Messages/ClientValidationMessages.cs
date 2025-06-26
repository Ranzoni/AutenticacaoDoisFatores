using System.ComponentModel;

namespace AutenticacaoDoisFatores.Domain.Shared.Messages
{
    public enum ClientValidationMessages
    {
        [Description("O nome do cliente não é válido. Ele deve conter entre 3 e 50 caracteres.")]
        InvalidName,
        [Description("O e-mail do cliente não é válido.")]
        InvalidEmail,
        [Description("O nome de domínio do cliente não é válido. Ele não pode conter caracteres especiais, acentos, espaços ou pontuações.")]
        InvalidDomainName,
        [Description("A chave de acesso gerada está inválida.")]
        InvalidAccessKey,
        [Description("O cliente não foi encontrado.")]
        ClientNotFound,
        [Description("O domínio informado já está cadastrado.")]
        DomainNameAlreadyRegistered,
        [Description("O e-mail informado já está cadastrado.")]
        EmailAlreadyRegistered,
        [Description("O link para a confirmação do cadastro não foi informado.")]
        ConfirmationLinkNotInformed,
        [Description("O token desta requisição é inválido.")]
        InvalidToken,
        [Description("Este cliente já foi ativado.")]
        ClientAlreadyActivated,
        [Description("Este cliente não está ativo!")]
        ClientNotActive
    }
}
