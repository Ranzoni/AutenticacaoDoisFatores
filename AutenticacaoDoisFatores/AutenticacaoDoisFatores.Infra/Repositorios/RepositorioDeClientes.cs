using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Infra.Contexto;

namespace AutenticacaoDoisFatores.Infra.Repositorios
{
    public class RepositorioDeClientes(CrudContexto contexto) : IRepositorioDeClientes
    {
        private readonly CrudContexto _contexto = contexto;

        public void Adicionar(Cliente entidade)
        {
            _contexto.Add(entidade);
        }

        public void Editar(Cliente entidade)
        {
            throw new NotImplementedException();
        }

        public void Excluir(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _contexto.SaveChangesAsync();
        }
    }
}
