﻿using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Mensagens;
using AutenticacaoDoisFatores.Dominio.Construtores;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;
using AutenticacaoDoisFatores.Dominio.Repositorios;
using Bogus;
using Moq;
using Moq.AutoMock;

namespace AutenticacaoDoisFatores.Testes.Dominio.Dominios
{
    public class DominioDeClientesTeste
    {
        private readonly Faker _faker = new();
        private readonly AutoMocker _mocker = new();

        [Fact]
        internal async Task DeveCriarCliente()
        {
            #region Preparação do teste

            var nomeParaTeste = _faker.Company.CompanyName();
            var emailParaTeste = _faker.Internet.Email();
            var dominioParaTeste = _faker.Internet.DomainWord();
                
            var cliente = new ConstrutorDeCliente()
                .ComNome(nomeParaTeste)
                .ComEmail(emailParaTeste)
                .ComNomeDominio(dominioParaTeste)
                .ConstruirNovoCliente();

            var dominio = _mocker.CreateInstance<DominioDeClientes>();

            #endregion Preparação do teste

            var retorno = await dominio.CriarClienteAsync(cliente);

            #region Verificação do teste

            Assert.NotNull(retorno);
            Assert.Equal(cliente, retorno);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Adicionar(cliente), Times.Once);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion Verificação do teste
        }

        [Fact]
        internal async Task DeveCriarDominioCliente()
        {
            #region Preparação do teste

            var nomeParaTeste = _faker.Company.CompanyName();
            var emailParaTeste = _faker.Internet.Email();
            var dominioParaTeste = _faker.Internet.DomainWord();

            var cliente = new ConstrutorDeCliente()
                .ComNome(nomeParaTeste)
                .ComEmail(emailParaTeste)
                .ComNomeDominio(dominioParaTeste)
                .ConstruirNovoCliente();

            var dominio = _mocker.CreateInstance<DominioDeClientes>();

            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.BuscarUnicoAsync(cliente.Id)).ReturnsAsync(cliente);

            #endregion Preparação do teste

            await dominio.CriarDominioDoClienteAsync(cliente.Id);

            #region Verificação do teste

            _mocker.Verify<IRepositorioDeClientes>(r => r.BuscarUnicoAsync(cliente.Id), Times.Once);
            _mocker.Verify<IRepositorioDeClientes>(r => r.CriarDominio(cliente.NomeDominio), Times.Once);

            #endregion Verificação do teste
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarDominioDeClienteInexistente()
        {
            #region Preparação do teste

            var idParaTeste = Guid.NewGuid();

            var dominio = _mocker.CreateInstance<DominioDeClientes>();

            #endregion Preparação do teste

            var excecao = await Assert.ThrowsAsync<ExcecoesCliente>(() => dominio.CriarDominioDoClienteAsync(idParaTeste));

            #region Verificação do teste

            Assert.Equal(MensagensCliente.ClienteNaoEncontrado.Descricao(), excecao.Message);
            _mocker.Verify<IRepositorioDeClientes>(r => r.BuscarUnicoAsync(idParaTeste), Times.Once);
            _mocker.Verify<IRepositorioDeClientes>(r => r.CriarDominio(It.IsAny<string>()), Times.Never);

            #endregion Verificação do teste
        }

        [Fact]
        internal async Task DeveRetornarExcecaoAoCriarClienteComNomeDominioJaCadastrado()
        {
            #region Preparação do teste

            var nomeParaTeste = _faker.Company.CompanyName();
            var emailParaTeste = _faker.Internet.Email();
            var dominioParaTeste = _faker.Internet.DomainWord();

            var cliente = new ConstrutorDeCliente()
                .ComNome(nomeParaTeste)
                .ComEmail(emailParaTeste)
                .ComNomeDominio(dominioParaTeste)
                .ConstruirNovoCliente();

            var dominio = _mocker.CreateInstance<DominioDeClientes>();
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.ExisteDominio(dominioParaTeste)).ReturnsAsync(true);

            #endregion Preparação do teste

            var excecao = await Assert.ThrowsAsync<ExcecoesCliente>(() => dominio.CriarClienteAsync(cliente));

            #region Verificação do teste

            Assert.Equal(MensagensCliente.NomeDominioJaCadastrado.Descricao(), excecao.Message);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Adicionar(It.IsAny<Cliente>()), Times.Never);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Never);

            #endregion Verificação do teste
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        internal async Task DeveRetornarVerdadeiroOuFalsoDominioCadastrado(bool dominioJaCadastrado)
        {
            #region Preparação do teste

            var nomeDominioParaTeste = _faker.Internet.DomainName();

            var dominio = _mocker.CreateInstance<DominioDeClientes>();
            _mocker.GetMock<IRepositorioDeClientes>().Setup(r => r.ExisteDominio(nomeDominioParaTeste)).ReturnsAsync(dominioJaCadastrado);

            #endregion Preparação do teste

            var retorno = await dominio.NomeDominioEstaCadastrado(nomeDominioParaTeste);

            #region Verificação do teste

            Assert.Equal(retorno, dominioJaCadastrado);
            _mocker.Verify<IRepositorioDeClientes>(r => r.ExisteDominio(nomeDominioParaTeste), Times.Once);

            #endregion Verificação do teste
        }
    }
}
