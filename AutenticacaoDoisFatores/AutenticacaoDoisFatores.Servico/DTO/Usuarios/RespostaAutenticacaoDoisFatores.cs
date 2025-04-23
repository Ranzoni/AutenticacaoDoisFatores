namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class RespostaAutenticacaoDoisFatores(string token, string? qrCode = null)
    {
        public string Token { get; } = token;
        public string? QrCode { get; } = qrCode;
    }
}
