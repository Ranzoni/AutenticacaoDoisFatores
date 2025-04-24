using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public class GerenciadorDeCodAutenticacao(IRepositorioDeCodigoDeAutenticacao repositorio)
    {
        private readonly IRepositorioDeCodigoDeAutenticacao _repositorio = repositorio;

        public async Task SalvarAsync(Guid idUsuario, string codigo)
        {
            if (codigo.EstaVazio())
                ExcecoesCodDeAutenticacao.CodigoAutenticacaoVazio();

            await _repositorio.SalvarAsync(idUsuario, codigo);
        }

        public async Task<string?> BuscarCodigoAsync(Guid idUsuario)
        {
            return await _repositorio.BuscarCodigoAsync(idUsuario);
        }
    }
}
