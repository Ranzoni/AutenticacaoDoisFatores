namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class EnvioEmailParaUsuario(string titulo, string mensagem, string htmlEmail)
    {
        public string Titulo { get; } = titulo;
        public string Mensagem { get; } = mensagem;
        public string HtmlEmail { get; } = htmlEmail;
    }
}
