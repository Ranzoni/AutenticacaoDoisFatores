using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Dominio.Dominios;

namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class NovosDadosUsuario(string nome, string nomeUsuario, string email, string senha, TipoDeAutenticacao? tipoDeAutenticacao)
    {
        private readonly string? _senhaDescriptografada = senha;

        public string Nome { get; } = nome;
        public string NomeUsuario { get; } = nomeUsuario;
        public string Email { get; } = email;
        public string Senha { get; } = Criptografia.CriptografarComSha512(senha);
        public TipoDeAutenticacao? TipoDeAutenticacao { get; } = tipoDeAutenticacao;

        public string SenhaDescriptografada()
        {
            return _senhaDescriptografada ?? "";
        }
    }
}
