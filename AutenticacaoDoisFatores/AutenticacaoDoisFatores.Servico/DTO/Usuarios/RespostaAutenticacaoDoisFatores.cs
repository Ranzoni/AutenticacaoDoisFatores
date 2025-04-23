namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class RespostaAutenticacaoDoisFatores(string token)
    {
        public string Token { get; } = token;
    }
}
