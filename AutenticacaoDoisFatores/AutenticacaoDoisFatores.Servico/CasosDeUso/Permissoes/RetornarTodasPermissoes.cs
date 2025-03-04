using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Servico.DTO.Permissoes;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Permissoes
{
    public static class RetornarTodasPermissoes
    {
        public static IEnumerable<PermissaoDisponivel> Executar()
        {
            var permissoes = Enum.GetValues<TipoDePermissao>()
                .Cast<TipoDePermissao>()
                .Select(tipo => new PermissaoDisponivel(tipo.Descricao() ?? "", tipo));

            return permissoes;
        }
    }
}
