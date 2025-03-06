using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Servico.DTO.Permissoes;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Permissoes
{
    public class RetornarPermissoes(DominioDePermissoes dominio)
    {
        private readonly DominioDePermissoes _dominio = dominio;

        public static IEnumerable<PermissaoDisponivel> RetornarTodas()
        {
            var permissoes = DominioDePermissoes.RetornarTodasPermissoes()
                .Select(tipo => new PermissaoDisponivel(tipo.Descricao() ?? "", tipo));

            return permissoes;
        }

        public async Task<IEnumerable<PermissaoDisponivel>> RetornarPorUsuarioAsync(Guid idUsuario)
        {
            var permissoes = await _dominio.RetornarPermissoes(idUsuario);

            return permissoes
                .Select(tipo => new PermissaoDisponivel(tipo.Descricao() ?? "", tipo));
        }
    }
}
