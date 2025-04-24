using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;

namespace AutenticacaoDoisFatores.Infra.Entidades
{
    internal class Permissao
    {
        internal Guid Id { get; private set; } = Guid.NewGuid();

        internal Guid IdUsuario { get; private set; }

        internal IEnumerable<TipoDePermissao> Permissoes { get; private set; } = [];

        internal DateTime DataCadastro { get; private set; }

        internal DateTime? DataAlteracao { get; private set; }

        internal Permissao(Guid idUsuario, IEnumerable<TipoDePermissao> permissoes)
        {
            IdUsuario = idUsuario;
            Permissoes = permissoes;
            DataCadastro = DateTime.Now;
        }
    }
}
