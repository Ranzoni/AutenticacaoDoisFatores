namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class DadosAutenticacao(string nomeUsuarioOuEmail, string senha)
    {
        public string NomeUsuarioOuEmail { get; } = nomeUsuarioOuEmail;
        public string Senha { get; } = senha;
    }
}
