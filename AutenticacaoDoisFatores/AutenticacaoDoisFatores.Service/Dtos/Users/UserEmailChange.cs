namespace AutenticacaoDoisFatores.Service.Dtos.Users
{
    public class UserEmailChange(string newEmail)
    {
        public string NewEmail { get; } = newEmail;
    }
}
