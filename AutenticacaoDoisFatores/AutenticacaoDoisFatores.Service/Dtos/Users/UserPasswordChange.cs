namespace AutenticacaoDoisFatores.Service.Dtos.Users
{
    public class UserPasswordChange(string newPassword)
    {
        public string NewPassword { get; } = newPassword;
    }
}
