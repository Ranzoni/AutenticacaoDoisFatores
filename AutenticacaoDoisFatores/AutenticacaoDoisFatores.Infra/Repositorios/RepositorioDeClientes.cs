using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Infra.Contexto;
using Microsoft.EntityFrameworkCore;

namespace AutenticacaoDoisFatores.Infra.Repositorios
{
    public class RepositorioDeClientes(CrudContexto contexto) : IRepositorioDeClientes
    {
        private readonly CrudContexto _contexto = contexto;

        #region Escrita

        public void Adicionar(Cliente entidade)
        {
            _contexto.Add(entidade);
        }

        public async Task CriarDominio(string nomeDominio)
        {
            var sql = $"CREATE SCHEMA IF NOT EXISTS {nomeDominio};";

            await _contexto.Database.ExecuteSqlRawAsync(sql);
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

        public async Task<bool> ExisteDominio(string nomeDominio)
        {
            return await _contexto.Clientes.AnyAsync(c => c.NomeDominio.ToLower().Trim().Equals(nomeDominio.ToLower().Trim()));
        }

        public async Task<bool> ExisteEmail(string email)
        {
            return await _contexto.Clientes.AnyAsync(c => c.Email.ToLower().Trim().Equals(email.ToLower().Trim()));
        }

        #endregion
    }
}
