namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class UsuarioAutenticado(UsuarioCadastrado usuario, string token)
    {
        public UsuarioCadastrado Usuario { get; } = usuario;
        public string Token { get; } = token;
    }
}
