﻿using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Infra.Contexto;
using Microsoft.EntityFrameworkCore;

namespace AutenticacaoDoisFatores.Infra.Repositorios
{
    public class RepositorioDeClientes(CrudContexto contexto) : IRepositorioDeClientes
    {
        private readonly CrudContexto _contexto = contexto;

        public void Adicionar(Cliente entidade)
        {
            _contexto.Add(entidade);
        }

        public Task<Cliente?> BuscarUnicoAsync(Guid id)
        {
            return _contexto.Clientes.FirstOrDefaultAsync(c => c.Id.Equals(id));
        }

        public async Task CriarDominio(string nomeDominio)
        {
            var sql = $"CREATE SCHEMA IF NOT EXISTS {nomeDominio};";

            await _contexto.Database.ExecuteSqlRawAsync(sql);
        }

        public void Editar(Cliente entidade)
        {
            throw new NotImplementedException();
        }

        public void Excluir(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExisteDominio(string nomeDominio)
        {
            return await _contexto.Clientes.AnyAsync(c => c.NomeDominio.Equals(nomeDominio));
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _contexto.SaveChangesAsync();
        }
    }
}
