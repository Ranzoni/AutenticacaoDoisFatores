namespace AutenticacaoDoisFatores.Domain.Services
{
    public interface IEmailService
    {
        public void Send(string to, string subject, string message);
    }
}
