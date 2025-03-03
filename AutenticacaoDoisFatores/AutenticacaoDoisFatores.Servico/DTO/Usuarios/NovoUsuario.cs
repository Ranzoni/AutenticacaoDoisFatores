using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Dominio.Dominios;

namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class NovoUsuario(string nome, string nomeUsuario, string email, string senha, IEnumerable<TipoDePermissao>? permissoes = null)
    {
        private readonly string _senhaDescriptografada = senha;
        private readonly IEnumerable<TipoDePermissao>? _permissoes = permissoes;

        public string Nome { get; } = nome;
        public string NomeUsuario { get; } = nomeUsuario;
        public string Email { get; } = email;
        public string Senha { get; } = Criptografia.CriptografarComSha512(senha);
        public IReadOnlyCollection<TipoDePermissao>? Permissoes
        {
            get
            {
                return _permissoes?.ToList();
            }
        }

        public string SenhaDescriptografada()
        {
            return _senhaDescriptografada;
        }
    }
}
