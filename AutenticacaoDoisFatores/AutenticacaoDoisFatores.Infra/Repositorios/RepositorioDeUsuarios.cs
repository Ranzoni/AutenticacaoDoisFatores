using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Dominio.Construtores;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Filtros;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Infra.Contexto;
using System.Data.Common;

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
                    (""Id"", ""Nome"", ""NomeUsuario"", ""Email"", ""Senha"", ""Celular"", ""ChaveSecreta"", ""DataCadastro"")
                VALUES
                    ('{entidade.Id}', '{entidade.Nome}', '{entidade.NomeUsuario}', '{entidade.Email}', '{entidade.Senha}', {(entidade.Celular is null ? "NULL" : entidade.Celular)}, '{Criptografia.CriptografarEmAes(entidade.ChaveSecreta)}', '{entidade.DataCadastro:yyyy-MM-dd HH:mm:ss}');";

            _contexto.PrepararComando(
                entidade: entidade,
                tipo: TipoComando.Inclusao,
                tabela: "Usuarios",
                sql: sql);
        }

        public void Adicionar(Usuario entidade, string dominio)
        {
            var sql = $@"
                INSERT INTO {dominio}.""Usuarios""
                    (""Id"", ""Nome"", ""NomeUsuario"", ""Email"", ""Senha"", ""DataCadastro"", ""Ativo"", ""EhAdmin"", ""ChaveSecreta"")
                VALUES
                    ('{entidade.Id}', '{entidade.Nome}', '{entidade.NomeUsuario}', '{entidade.Email}', '{entidade.Senha}', '{entidade.DataCadastro:yyyy-MM-dd HH:mm:ss}', {entidade.Ativo}, {entidade.EhAdmin}, '{Criptografia.CriptografarEmAes(entidade.ChaveSecreta)}');";

            _contexto.PrepararComando(
                entidade: entidade,
                tipo: TipoComando.Inclusao,
                tabela: "Usuarios",
                sql: sql,
                nomeDominio: dominio);
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
                    ""Celular"" = {(entidade.Celular is null ? "NULL" : entidade.Celular)},
                    ""Ativo"" = {entidade.Ativo},
                    ""DataAlteracao"" = '{entidade.DataAlteracao:yyyy-MM-dd HH:mm:ss}'";

            if (entidade.TipoDeAutenticacao is null)
                sql += $@",""TipoDeAutenticacao"" = NULL";
            else
                sql += $@",""TipoDeAutenticacao"" = {(int)entidade.TipoDeAutenticacao}";

            if (entidade.DataUltimoAcesso is not null)
                sql += $@",""DataUltimoAcesso"" = '{entidade.DataUltimoAcesso:yyyy-MM-dd HH:mm:ss}'";

            sql += $@" WHERE
                ""Id"" = '{entidade.Id}';";

            _contexto.PrepararComando(
                entidade: entidade,
                tipo: TipoComando.Alteracao,
                tabela: "Usuarios",
                sql: sql);
        }

        public void Excluir(Usuario entidade)
        {
            var sql = $@"
                DELETE FROM
                    {_contexto.NomeDominio}.""Usuarios""
                WHERE
                    ""Id"" = '{entidade.Id}';";

            _contexto.PrepararComando(
                entidade: entidade,
                tipo: TipoComando.Exclusao,
                tabela: "Usuarios",
                sql: sql);
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
                    {MontarCamposDoUsuario()}
                FROM
                    {_contexto.NomeDominio}.""Usuarios"" u
                WHERE
                    u.""Id"" = '{id}'";

            return await _contexto.LerUnicoAsync(sql, ConstruirUsuario);
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

        public async Task<Usuario?> BuscarPorNomeUsuarioAsync(string nomeUsuario)
        {
            var sql = $@"
                SELECT
                    {MontarCamposDoUsuario()}
                FROM
                    {_contexto.NomeDominio}.""Usuarios"" u
                WHERE
                    u.""NomeUsuario"" = '{nomeUsuario}'";

            return await _contexto.LerUnicoAsync(sql, ConstruirUsuario);
        }

        public async Task<Usuario?> BuscarPorEmailAsync(string email)
        {
            var sql = $@"
                SELECT
                    {MontarCamposDoUsuario()}
                FROM
                    {_contexto.NomeDominio}.""Usuarios"" u
                WHERE
                    u.""Email"" = '{email}'";

            return await _contexto.LerUnicoAsync(sql, ConstruirUsuario);
        }

        public async Task<bool> ExisteNomeUsuarioAsync(string nomeUsuario, string dominio)
        {
            var sql = $@"
                SELECT
                    COUNT(1) > 0
                FROM
                    {dominio}.""Usuarios"" u
                WHERE
                    u.""NomeUsuario"" = '{nomeUsuario}'";

            return await _contexto.ConsultaEhVerdadeiraAsync(sql);
        }

        public async Task<bool> ExisteEmailAsync(string email, string dominio)
        {
            var sql = $@"
                SELECT
                    COUNT(1) > 0
                FROM
                    {dominio}.""Usuarios"" u
                WHERE
                    u.""Email"" = '{email}'";

            return await _contexto.ConsultaEhVerdadeiraAsync(sql);
        }

        public async Task<bool> EhAdmAsync(Guid id)
        {
            var sql = $@"
                SELECT
                    COUNT(1) > 0
                FROM
                    {_contexto.NomeDominio}.""Usuarios"" u
                WHERE
                    u.""Id"" = '{id}' AND
                    u.""EhAdmin"" = true";

            return await _contexto.ConsultaEhVerdadeiraAsync(sql);
        }

        public async Task<Usuario?> BuscarUsuarioPorDominioAsync(Guid id, string dominio)
        {
            var sql = $@"
                SELECT
                    {MontarCamposDoUsuario()}
                FROM
                    {dominio}.""Usuarios"" u
                WHERE
                    u.""Id"" = '{id}'";

            return await _contexto.LerUnicoAsync(sql, ConstruirUsuario);
        }

        public async Task<IEnumerable<Usuario>> BuscarVariosAsync(FiltroDeUsuarios filtros)
        {
            var qtdParaPular = (filtros.Pagina - 1) * filtros.QtdPorPagina;

            var sql = $@"
                SELECT
                    {MontarCamposDoUsuario()}
                FROM
                    {_contexto.NomeDominio}.""Usuarios"" u";

            var filtrosSql = "";

            if (!filtros.Nome!.EstaVazio())
                filtrosSql = AdicionarCondicaoSql(filtrosSql, $@"LOWER(u.""Nome"") like '%{filtros.Nome!.ToLower()}%'");

            if (!filtros.NomeUsuario!.EstaVazio())
                filtrosSql = AdicionarCondicaoSql(filtrosSql, $@"LOWER(u.""NomeUsuario"") like '%{filtros.NomeUsuario!.ToLower()}%'");

            if (filtros.Ativo.HasValue)
                filtrosSql = AdicionarCondicaoSql(filtrosSql, $@"u.""Ativo"" = {filtros.Ativo}");

            if (filtros.DataUltimoAcessoDe.HasValue)
                filtrosSql = AdicionarCondicaoSql(filtrosSql, $@"u.""DataUltimoAcesso""::DATE >= '%{filtros.DataUltimoAcessoDe:yyyy-MM-dd}%'");

            if (filtros.DataUltimoAcessoAte.HasValue)
                filtrosSql = AdicionarCondicaoSql(filtrosSql, $@"(u.""DataUltimoAcesso"" IS NULL OR u.""DataUltimoAcesso""::DATE <= '%{filtros.DataUltimoAcessoAte:yyyy-MM-dd}%')");

            if (filtros.DataCadastroDe.HasValue)
                filtrosSql = AdicionarCondicaoSql(filtrosSql, $@"u.""DataCadastro""::DATE >= '{filtros.DataCadastroDe:yyyy-MM-dd}'");

            if (filtros.DataCadastroAte.HasValue)
                filtrosSql = AdicionarCondicaoSql(filtrosSql, $@"u.""DataCadastro""::DATE <= '{filtros.DataCadastroAte:yyyy-MM-dd}'");

            if (filtros.DataAlteracaoDe.HasValue)
                filtrosSql = AdicionarCondicaoSql(filtrosSql, $@"u.""DataAlteracao""::DATE >= '{filtros.DataAlteracaoDe:yyyy-MM-dd}'");

            if (filtros.DataAlteracaoAte.HasValue)
                filtrosSql = AdicionarCondicaoSql(filtrosSql, $@"(u.""DataAlteracao"" IS NULL OR u.""DataAlteracao""::DATE <= '{filtros.DataAlteracaoAte:yyyy-MM-dd}')");

            if (filtros.EhAdmin.HasValue)
                filtrosSql = AdicionarCondicaoSql(filtrosSql, $@"u.""EhAdmin"" = {filtros.EhAdmin}");

            sql += $@"{filtrosSql}
                ORDER BY u.""Id""
                LIMIT {filtros.QtdPorPagina}
                OFFSET {qtdParaPular}";

            return await _contexto.LerVariosAsync(sql,
                leitor =>
                {
                    var listaDeUsuarios = new List<Usuario>();

                    while (leitor.Read())
                    {
                        var usuario = ConstruirUsuario(leitor);
                        listaDeUsuarios.Add(usuario);
                    }

                    return listaDeUsuarios;
                });
        }

        private static string MontarCamposDoUsuario()
        {
            return @"u.""Id"",
                u.""Nome"",
                u.""NomeUsuario"",
                u.""Email"",
                u.""Senha"",
                u.""Ativo"",
                u.""DataUltimoAcesso"",
                u.""DataCadastro"",
                u.""DataAlteracao"",
                u.""EhAdmin"",
                u.""TipoDeAutenticacao"",
                u.""Celular"",
                u.""ChaveSecreta""";
        }

        private static Usuario ConstruirUsuario(DbDataReader leitor)
        {
            return new ConstrutorDeUsuario()
                .ComId(leitor.GetGuid(0))
                .ComNome(leitor.GetString(1))
                .ComNomeUsuario(leitor.GetString(2))
                .ComEmail(leitor.GetString(3))
                .ComSenha(leitor.GetString(4))
                .ComAtivo(leitor.GetBoolean(5))
                .ComDataUltimoAcesso(leitor.IsDBNull(6) ? null : leitor.GetDateTime(6))
                .ComDataCadastro(leitor.GetDateTime(7))
                .ComDataAlteracao(leitor.IsDBNull(8) ? null : leitor.GetDateTime(8))
                .ComEhAdmin(leitor.GetBoolean(9))
                .ComTipoDeAutenticacao(leitor.IsDBNull(10) ? null : (TipoDeAutenticacao)leitor.GetInt16(10))
                .ComCelular(leitor.IsDBNull(11) ? null : leitor.GetInt64(11))
                .ComChaveSecreta(Criptografia.DescriptografarEmAes(leitor.GetString(12)))
                .ConstruirCadastrado();
        }

        private static string AdicionarCondicaoSql(string sql, string condicao)
        {
            if (sql.EstaVazio())
                sql += @"
                    WHERE ";
            else
                sql += @"
                    AND ";

            return sql + condicao;
        }

        #endregion
    }
}
