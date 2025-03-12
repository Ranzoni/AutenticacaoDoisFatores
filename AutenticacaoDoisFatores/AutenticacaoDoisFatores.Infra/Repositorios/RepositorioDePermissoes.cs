using AutenticacaoDoisFatores.Dominio.Compartilhados.Permissoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Infra.Contexto;
using AutenticacaoDoisFatores.Infra.Entidades;
using AutenticacaoDoisFatores.Infra.Utilitarios;

namespace AutenticacaoDoisFatores.Infra.Repositorios
{
    public class RepositorioDePermissoes(ContextoPermissoes contexto) : IRepositorioDePermissoes
    {
        private readonly ContextoPermissoes _contexto = contexto;

        public async Task AdicionarAsync(Guid idUsuario, IEnumerable<TipoDePermissao> permissoes)
        {
            var permissao = new Permissao(idUsuario, permissoes);

            await _contexto.Permissoes.AdicionarAsync(permissao);

            var auditoria = _contexto.MontarAuditoria(typeof(Permissao), idUsuario, AcoesDeAuditoria.Inclusao, permissao);
            if (auditoria is not null)
                await _contexto.Audiorias.AdicionarAsync(auditoria);
        }

        public async Task<IEnumerable<TipoDePermissao>> RetornarPorUsuarioAsync(Guid idUsuario)
        {
            var permissao = await _contexto.Permissoes
                .BuscarUnicoAsync(p => p.IdUsuario.Equals(idUsuario));

            return permissao?.Permissoes ?? [];
        }

        public async Task EditarAsync(Guid idUsuario, IEnumerable<TipoDePermissao> permissoes)
        {
            await _contexto.Permissoes.EditarAsync
            (
                filtroExpressao: p => p.IdUsuario == idUsuario,
                campo: p => p.Permissoes,
                valor: permissoes
            );

            var auditoria = _contexto.MontarAuditoria(typeof(Permissao), idUsuario, AcoesDeAuditoria.Modificacao, new { permissoes });
            if (auditoria is not null)
                await _contexto.Audiorias.AdicionarAsync(auditoria);
        }
    }
}
