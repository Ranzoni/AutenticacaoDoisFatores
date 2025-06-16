using AutenticacaoDoisFatores.Domain.Entities;

namespace AutenticacaoDoisFatores.Service.UseCases.Users.Authenticators
{
    internal interface IAuthType
    {
        Task<object?> ExecuteAsync(User user);
    }
}
