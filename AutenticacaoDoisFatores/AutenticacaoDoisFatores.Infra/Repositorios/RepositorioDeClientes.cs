using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Infra.Compartilhados.Migradores;
using AutenticacaoDoisFatores.Infra.Contexto;
using Microsoft.EntityFrameworkCore;

namespace AutenticacaoDoisFatores.Infra.Repositorios
{
    public class RepositorioDeClientes(ContextoPadrao contexto, IMigrador migrador) : IRepositorioDeClientes
    {
        private readonly ContextoPadrao _contexto = contexto;

        #region Escrita

        public void Adicionar(Cliente entidade)
        {
            _contexto.Clientes.Select(s => new { s.Nome });

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

        public void Excluir(Guid id)
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
            var cliente = await _contexto.Clientes.FirstOrDefaultAsync(c => c.ChaveAcesso.Equals(chave));
            return cliente?.NomeDominio;
        }

        #endregion
    }
}
