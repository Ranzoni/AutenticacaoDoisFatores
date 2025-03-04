using AutenticacaoDoisFatores.Dominio.Construtores;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Infra.Contexto;
using System.Numerics;

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
                    (""Id"", ""Nome"", ""NomeUsuario"", ""Email"", ""Senha"", ""DataCadastro"", ""Ativo"", ""EhAdmin"")
                VALUES
                    ('{entidade.Id}', '{entidade.Nome}', '{entidade.NomeUsuario}', '{entidade.Email}', '{entidade.Senha}', '{entidade.DataCadastro:yyyy-MM-dd HH:mm:ss}', {entidade.Ativo}, {entidade.EhAdmin});";

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
                    ""Ativo"" = {entidade.Ativo},
                    ""DataAlteracao"" = '{entidade.DataAlteracao:yyyy-MM-dd HH:mm:ss}'
                WHERE
                    ""Id"" = '{entidade.Id}';";

            _contexto.PrepararComando(
                entidade: entidade,
                tipo: TipoComando.Alteracao,
                tabela: "Usuarios",
                sql: sql);
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
                    u.""DataAlteracao"",
                    u.""EhAdmin""
                FROM
                    {_contexto.NomeDominio}.""Usuarios"" u
                WHERE
                    u.""Id"" = '{id}'";

            return await _contexto.LerUnicoAsync(sql,
                leitor => new ConstrutorDeUsuario()
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
                    .ConstruirCadastrado());
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
                    u.""Id"",
                    u.""Nome"",
                    u.""NomeUsuario"",
                    u.""Email"",
                    u.""Senha"",
                    u.""Ativo"",
                    u.""DataUltimoAcesso"",
                    u.""DataCadastro"",
                    u.""DataAlteracao"",
                    u.""EhAdmin""
                FROM
                    {_contexto.NomeDominio}.""Usuarios"" u
                WHERE
                    u.""NomeUsuario"" = '{nomeUsuario}'";

            return await _contexto.LerUnicoAsync(sql,
                leitor => new ConstrutorDeUsuario()
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
                    .ConstruirCadastrado());
        }

        public async Task<Usuario?> BuscarPorEmailAsync(string email)
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
                    u.""DataAlteracao"",
                    u.""EhAdmin""
                FROM
                    {_contexto.NomeDominio}.""Usuarios"" u
                WHERE
                    u.""Email"" = '{email}'";

            return await _contexto.LerUnicoAsync(sql,
                leitor => new ConstrutorDeUsuario()
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
                    .ConstruirCadastrado());
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

        #endregion
    }
}
