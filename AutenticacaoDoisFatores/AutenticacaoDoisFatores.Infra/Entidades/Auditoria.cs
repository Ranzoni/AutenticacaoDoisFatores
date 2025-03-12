using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Infra.Utilitarios;

namespace AutenticacaoDoisFatores.Infra.Entidades
{
    internal class Auditoria
    {
        internal Guid Id { get; } = Guid.NewGuid();
        internal Guid IdEntidade { get; } = Guid.Empty;
        internal string Acao { get; } = "";
        internal string Tabela { get; } = "";
        internal object? Detalhes { get; } = null;
        internal DateTime Data { get; } = DateTime.Now;

        internal Auditoria(AcoesDeAuditoria acao, Guid idEntidade, string tabela, object detalhes)
        {
            Acao = acao.Descricao() ?? "";
            IdEntidade = idEntidade;
            Tabela = tabela;
            Detalhes = detalhes;
        }
    }
}
