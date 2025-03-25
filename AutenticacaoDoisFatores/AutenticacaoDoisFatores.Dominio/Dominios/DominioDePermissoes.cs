using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public class DominioDePermissoes(IRepositorioDePermissoes repositorio)
    {
        private readonly IRepositorioDePermissoes _repositorio = repositorio;

        public async Task AdicionarAsync(Guid idUsuario, IEnumerable<TipoDePermissao> permissoesParaIncluir)
        {
            var permissoesJaInclusas = await RetornarPermissoesAsync(idUsuario) ?? [];
            if (permissoesJaInclusas.Any())
            {
                var novasPermissoes = permissoesParaIncluir.Except(permissoesJaInclusas);
                var permissoesAtualizadas = permissoesJaInclusas.Concat(novasPermissoes);
                await _repositorio.EditarAsync(idUsuario, permissoesAtualizadas);
            }
            else
                await _repositorio.AdicionarAsync(idUsuario, permissoesParaIncluir);
        }

        public async Task RemoverAsync(Guid idUsuario, IEnumerable<TipoDePermissao> permissoesParaRemover)
        {
            var permissoesJaInclusas = await RetornarPermissoesAsync(idUsuario) ?? [];
            if (!permissoesJaInclusas.Any())
                return;

            var permissoesRestantes = permissoesJaInclusas.Except(permissoesParaRemover);

            await _repositorio.EditarAsync(idUsuario, permissoesRestantes);
        }

        public async Task<IEnumerable<TipoDePermissao>> RetornarPermissoesAsync(Guid idUsuario)
        {
            return await _repositorio.RetornarPorUsuarioAsync(idUsuario);
        }

        public static IEnumerable<TipoDePermissao> RetornarTodasPermissoes()
        {
            return Enum.GetValues<TipoDePermissao>();
        }
    }
}
