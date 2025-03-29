namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class NovosDadosUsuario(string? nome, string? nomeUsuario)
    {
        public string? Nome { get; } = nome;
        public string? NomeUsuario { get; } = nomeUsuario;
    }
}
