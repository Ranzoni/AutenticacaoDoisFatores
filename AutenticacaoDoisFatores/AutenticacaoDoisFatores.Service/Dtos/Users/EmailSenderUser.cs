namespace AutenticacaoDoisFatores.Service.Dtos.Users
{
    public class EmailSenderUser(string subject, string message, string htmlEmail)
    {
        public string Subject { get; } = subject;
        public string Message { get; } = message;
        public string HtmlEmail { get; } = htmlEmail;
    }
}
