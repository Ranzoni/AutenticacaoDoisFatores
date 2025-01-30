using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;

namespace AutenticacaoDoisFatores.Dominio.Dominios
{
    public partial class DominioDeClientes(IRepositorioDeClientes repositorio)
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
                ?? throw new ExcecoesCliente(MensagensValidacaoCliente.ClienteNaoEncontrado);

            await _repositorio.CriarDominio(cliente.NomeDominio);
        }

        public async Task<Cliente> AlterarClienteAsync(Cliente cliente)
        {
            _repositorio.Editar(cliente);
            await _repositorio.SalvarAlteracoesAsync();

            return cliente;
        }

        #endregion Escrita

        #region Leitura

        public async Task<Cliente?> BuscarClienteAsync(Guid id)
        {
            return await _repositorio.BuscarUnicoAsync(id);
        }

        public async Task<bool> EmailEstaCadastradoAsync(string email)
        {
            return await _repositorio.ExisteEmail(email);
        }

        public async Task<bool> NomeDominioEstaCadastradoAsync(string nomeDominio)
        {
            return await _repositorio.ExisteDominio(nomeDominio);
        }

        public async Task<Cliente?> BuscarPorEmailAsync(string email)
        {
            return await _repositorio.BuscarPorEmailAsync(email);
        }

        #endregion Leitura
    }

    #region Validador

    public partial class DominioDeClientes
    {
        private async Task ValidarCriacaoClienteAsync(Cliente cliente)
        {
            var existeEmail = await _repositorio.ExisteEmail(cliente.Email);
            if (existeEmail)
                ExcecoesCliente.EmailJaCadastrado();

            var existeDominio = await _repositorio.ExisteDominio(cliente.NomeDominio);
            if (existeDominio)
                ExcecoesCliente.NomeDominioJaCadastrado();
        }
    }

    #endregion
}
