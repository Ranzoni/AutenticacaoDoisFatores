using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using AutenticacaoDoisFatores.Infra.Contexto;

namespace AutenticacaoDoisFatores.Infra.Repositorios
{
    public class RepositorioDeUsuarios(object contexto) : IRepositorioDeUsuarios
    {
        private readonly object _contexto = contexto;

        #region Escrita

        public void Adicionar(Usuario entidade)
        {
            throw new NotImplementedException();
        }

        public void Editar(Usuario entidade)
        {
            throw new NotImplementedException();
        }

        public void Excluir(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task SalvarAlteracoesAsync()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Leitura

        public Task<Usuario?> BuscarUnicoAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExisteEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExisteNomeUsuarioAsync(string nomeUsuario)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
