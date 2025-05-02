namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class EnvioEmailAtivacaoUsuario(string link)
    {
        public string Link { get; } = link;
    }
}
