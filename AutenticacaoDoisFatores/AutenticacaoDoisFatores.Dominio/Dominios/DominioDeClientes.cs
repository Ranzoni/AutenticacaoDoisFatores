using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public class DominioDeClientes(IRepositorioDeClientes repositorio)
    {
        private readonly IRepositorioDeClientes _repositorio = repositorio;

        #region Escrita

        public async Task<Cliente> CriarClienteAsync(Cliente cliente)
        {
            await ValidarCriacaoClienteAsync(cliente);

            _repositorio.Adicionar(cliente);
            await _repositorio.SalvarAlteracoesAsync();

            return cliente;
        }

        public async Task CriarDominioDoClienteAsync(Guid idCliente)
        {
            var cliente = await _repositorio.BuscarUnicoAsync(idCliente)
                ?? throw new ExcecoesCliente(MensagensCliente.ClienteNaoEncontrado);

            await _repositorio.CriarDominio(cliente.NomeDominio);
        }

        private async Task ValidarCriacaoClienteAsync(Cliente cliente)
        {
            var existeEmail = await EmailEstaCadastradoAsync(cliente.Email);
            if (existeEmail)
                ExcecoesCliente.EmailJaCadastrado();

            var existeDominio = await NomeDominioEstaCadastradoAsync(cliente.NomeDominio);
            if (existeDominio)
                ExcecoesCliente.NomeDominioJaCadastrado();
        }

        #endregion Escrita

        #region Leitura

        public async Task<bool> EmailEstaCadastradoAsync(string email)
        {
            return await _repositorio.ExisteEmail(email);
        }

        public async Task<bool> NomeDominioEstaCadastradoAsync(string nomeDominio)
        {
            return await _repositorio.ExisteDominio(nomeDominio);
        }

        #endregion Leitura
    }
}
