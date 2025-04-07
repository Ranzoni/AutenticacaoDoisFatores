using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Infra.Utilitarios.Migradores;
using AutenticacaoDoisFatores.Infra.Contexto;
using Microsoft.EntityFrameworkCore;
using AutenticacaoDoisFatores.Dominio.Filtros;
using AutenticacaoDoisFatores.Dominio.Compartilhados;

namespace AutenticacaoDoisFatores.Infra.Repositorios
{
    public class RepositorioDeClientes(ContextoPadrao contexto, IMigrador migrador) : IRepositorioDeClientes
    {
        private readonly ContextoPadrao _contexto = contexto;

        #region Escrita

        public void Adicionar(Cliente entidade)
        {
            _contexto.Add(entidade);
        }

        public async Task CriarDominio(string nomeDominio)
        {
            var sql = $"CREATE SCHEMA IF NOT EXISTS {nomeDominio};";

            await _contexto.Database.ExecuteSqlRawAsync(sql);
            await migrador.AplicarMigracoesAsync(nomeDominio);
        }

        public void Editar(Cliente entidade)
        {
            _contexto.Update(entidade);
        }

        public void Excluir(Cliente entidade)
        {
            throw new NotImplementedException();
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _contexto.SaveChangesAsync();
        }

        #endregion

        #region Leitura

        public Task<Cliente?> BuscarUnicoAsync(Guid id)
        {
            return _contexto.Clientes.FirstOrDefaultAsync(c => c.Id.Equals(id));
        }

        public async Task<bool> ExisteDominioAsync(string nomeDominio)
        {
            return await _contexto.Clientes.AnyAsync(c => c.NomeDominio.ToLower().Trim().Equals(nomeDominio.ToLower().Trim()));
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _contexto.Clientes.AnyAsync(c => c.Email.ToLower().Trim().Equals(email.ToLower().Trim()));
        }

        public async Task<Cliente?> BuscarPorEmailAsync(string email)
        {
            return await _contexto.Clientes.FirstOrDefaultAsync(c => c.Email.Equals(email));
        }

        public async Task<string?> RetornarNomeDominioAsync(string chave)
        {
            var cliente = await _contexto.Clientes.FirstOrDefaultAsync(c => c.Ativo && c.ChaveAcesso.Equals(chave));
            return cliente?.NomeDominio;
        }

        public async Task<IEnumerable<Cliente>> BuscarVariosAsync(FiltroDeClientes filtros)
        {
            var qtdParaPular = (filtros.Pagina - 1) * filtros.QtdPorPagina;

            IQueryable<Cliente> consulta = _contexto.Clientes;

            if (!filtros.Nome!.EstaVazio())
                consulta = consulta.Where(c => c.Nome.ToLower().Contains(filtros.Nome!.ToLower()));

            if (!filtros.Email!.EstaVazio())
                consulta = consulta.Where(c => c.Email.ToLower().Contains(filtros.Email!.ToLower()));

            if (!filtros.NomeDominio!.EstaVazio())
                consulta = consulta.Where(c => c.NomeDominio.ToLower().Contains(filtros.NomeDominio!.ToLower()));

            if (filtros.Ativo.HasValue)
                consulta = consulta.Where(c => c.Ativo.Equals(filtros.Ativo));

            if (filtros.DataCadastroDe.HasValue)
                consulta = consulta.Where(c => c.DataCadastro.Date >= filtros.DataCadastroDe.Value.Date);

            if (filtros.DataCadastroAte.HasValue)
                consulta = consulta.Where(c => c.DataCadastro.Date <= filtros.DataCadastroAte.Value.Date);

            if (filtros.DataAlteracaoDe.HasValue)
                consulta = consulta.Where(c => c.DataAlteracao!.Value.Date >= filtros.DataAlteracaoDe.Value.Date);

            if (filtros.DataAlteracaoAte.HasValue)
                consulta = consulta.Where(c => c.DataAlteracao!.Value.Date <= filtros.DataAlteracaoAte.Value.Date);

            return await consulta
                .Skip(qtdParaPular)
                .Take(filtros.QtdPorPagina)
                .ToListAsync();
        }

        #endregion
    }
}
