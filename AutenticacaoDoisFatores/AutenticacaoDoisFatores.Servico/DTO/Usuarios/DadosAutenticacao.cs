namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class DadosAutenticacao(string? nomeUsuario, string? email, string senha)
    {
        public string? NomeUsuario { get; } = nomeUsuario;
        public string? Email { get; } = email;
        public string Senha { get; } = senha;
    }
}
