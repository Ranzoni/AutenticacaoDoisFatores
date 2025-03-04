using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Dominio.Dominios;

namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class NovoUsuario(string nome, string nomeUsuario, string email, string senha, IEnumerable<TipoDePermissao>? permissoes = null)
    {
        private readonly string _senhaDescriptografada = senha;

        public string Nome { get; } = nome;
        public string NomeUsuario { get; } = nomeUsuario;
        public string Email { get; } = email;
        public string Senha { get; } = Criptografia.CriptografarComSha512(senha);
        public IEnumerable<TipoDePermissao>? Permissoes { get; } = permissoes;

        public string SenhaDescriptografada()
        {
            return _senhaDescriptografada;
        }
    }
}
