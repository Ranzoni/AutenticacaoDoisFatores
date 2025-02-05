namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class NovoUsuario(string nome, string nomeUsuario, string email, string senha)
    {
        public string Nome { get; } = nome;
        public string NomeUsuario { get; } = nomeUsuario;
        public string Email { get; } = email;
        public string Senha { get; } = senha;
    }
}
