using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Servico.DTO.Permissoes;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Permissoes
{
    public static class RetornarTodasPermissoes
    {
        public static IEnumerable<PermissaoDisponivel> Executar()
        {
            var permissoes = DominioDePermissoes.RetornarTodasPermissoes()
                .Select(tipo => new PermissaoDisponivel(tipo.Descricao() ?? "", tipo));

            return permissoes;
        }
    }
}
