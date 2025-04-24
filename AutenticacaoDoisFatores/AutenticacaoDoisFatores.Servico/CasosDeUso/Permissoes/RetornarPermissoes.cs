using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Servico.DTO.Permissoes;

namespace AutenticacaoDoisFatores.Servico.CasosDeUso.Permissoes
{
    public class RetornarPermissoes(DominioDePermissoes dominio, DominioDeUsuarios usuarios)
    {
        private readonly DominioDePermissoes _dominio = dominio;
        private readonly DominioDeUsuarios _usuarios = usuarios;

        public static IEnumerable<PermissaoDisponivel> RetornarTodas()
        {
            var permissoes = DominioDePermissoes.RetornarTodasPermissoes()
                .Select(tipo => new PermissaoDisponivel(tipo.Descricao() ?? "", tipo));

            return permissoes;
        }

        public async Task<IEnumerable<PermissaoDisponivel>> RetornarPorUsuarioAsync(Guid idUsuario)
        {
            if (await _usuarios.EhAdmAsync(idUsuario))
                return RetornarTodas();

            var permissoes = await _dominio.RetornarPermissoesAsync(idUsuario);

            return permissoes
                .Select(tipo => new PermissaoDisponivel(tipo.Descricao() ?? "", tipo));
        }
    }
}
