using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Infra.Contexto;

namespace AutenticacaoDoisFatores.Infra.Repositorios
{
    public class RepositorioDeUsuarios(ContextoCliente contexto) : IRepositorioDeUsuarios
    {
        private readonly ContextoCliente _contexto = contexto;

        #region Escrita

        public void Adicionar(Usuario entidade)
        {
            var sql = $@"
                INSERT INTO {_contexto.NomeDominio}.""Usuarios""
                    (""Id"", ""Nome"", ""NomeUsuario"", ""Email"", ""Senha"", ""DataCadastro"")
                VALUES
                    ('{entidade.Id}', '{entidade.Nome}', '{entidade.NomeUsuario}', '{entidade.Email}', '{entidade.Senha}', '{entidade.DataCadastro:yyyy-MM-dd HH:mm:ss}');";

            _contexto.PrepararComando(sql);
        }

        public void Editar(Usuario entidade)
        {
            var sql = $@"
                UPDATE {_contexto.NomeDominio}.""Usuarios""
                SET
                    ""Nome"" = '{entidade.Nome}',
                    ""NomeUsuario"" = '{entidade.NomeUsuario}',
                    ""Email"" = '{entidade.Email}',
                    ""Senha"" = '{entidade.Senha}',
                    ""Ativo"" = {entidade.Ativo},
                    ""DataAlteracao"" = '{entidade.DataAlteracao:yyyy-MM-dd HH:mm:ss}'
                WHERE
                    ""Id"" = '{entidade.Id}';";

            _contexto.PrepararComando(sql);
        }

        public void Excluir(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _contexto.ExecutarComandosAsync();
        }

        #endregion

        #region Leitura

        public async Task<Usuario?> BuscarUnicoAsync(Guid id)
        {
            var sql = $@"
                SELECT
                    u.""Id"",
                    u.""Nome"",
                    u.""NomeUsuario"",
                    u.""Email"",
                    u.""Senha"",
                    u.""Ativo"",
                    u.""DataUltimoAcesso"",
                    u.""DataCadastro"",
                    u.""DataAlteracao""
                FROM
                    {_contexto.NomeDominio}.""Usuarios"" u
                WHERE
                    u.""Id"" = '{id}'";

            return await _contexto.LerUnicoAsync(sql,
                leitor => new Usuario
                (
                    id: leitor.GetGuid(0),
                    nome: leitor.GetString(1),
                    nomeUsuario: leitor.GetString(2),
                    email: leitor.GetString(3),
                    senha: leitor.GetString(4),
                    ativo: leitor.GetBoolean(5),
                    dataUltimoAcesso: leitor.IsDBNull(6) ? null : leitor.GetDateTime(6),
                    dataCadastro: leitor.GetDateTime(7),
                    dataAlteracao: leitor.IsDBNull(8) ? null : leitor.GetDateTime(8)
                ));
        }

        public async Task<bool> ExisteEmailAsync(string email, Guid? id = null)
        {
            var sql = $@"
                SELECT
                    COUNT(1) > 0
                FROM
                    {_contexto.NomeDominio}.""Usuarios"" u
                WHERE
                    u.""Email"" = '{email}'";

            if (id is not null)
                sql += $@" AND ""Id"" != '{id}'";

            return await _contexto.ConsultaEhVerdadeiraAsync(sql);
        }

        public async Task<bool> ExisteNomeUsuarioAsync(string nomeUsuario, Guid? id = null)
        {
            var sql = $@"
                SELECT
                    COUNT(1) > 0
                FROM
                    {_contexto.NomeDominio}.""Usuarios"" u
                WHERE
                    u.""NomeUsuario"" = '{nomeUsuario}'";

            if (id is not null)
                sql += $@" AND ""Id"" != '{id}'";

            return await _contexto.ConsultaEhVerdadeiraAsync(sql);
        }

        #endregion
    }
}
