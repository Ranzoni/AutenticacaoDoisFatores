using System.ComponentModel;

namespace AutenticacaoDoisFatores.Domain.Shared.Messages
{
    public enum AuthCodeValidationMessages
    {
        [Description("O código de autenticação está vazio")]
        EmptyAuthCode
    }
}
