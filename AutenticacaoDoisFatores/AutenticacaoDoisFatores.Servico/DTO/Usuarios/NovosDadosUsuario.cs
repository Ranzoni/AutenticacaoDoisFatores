using AutenticacaoDoisFatores.Dominio.Dominios;

namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class NovosDadosUsuario(string? nome, string? nomeUsuario, string? email, string? senha)
    {
        private readonly string? _senhaDescriptografada = senha;

        public string? Nome { get; } = nome;
        public string? NomeUsuario { get; } = nomeUsuario;
        public string? Email { get; } = email;
        public string? Senha { get; } = senha is not null ? Criptografia.CriptografarComSha512(senha) : null;

        public string SenhaDescriptografada()
        {
            return _senhaDescriptografada ?? "";
        }
    }
}
