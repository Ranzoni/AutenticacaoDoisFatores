﻿using AutenticacaoDoisFatores.Dominio.Construtores;
using AutenticacaoDoisFatores.Dominio.Dominios;
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
                
            var cliente = new ConstrutorDeCliente()
                .ComNome(nomeParaTeste)
                .ComEmail(emailParaTeste)
                .ConstruirNovoCliente();

            var dominio = _mocker.CreateInstance<DominioDeClientes>();

            #endregion Preparação do teste

            var retorno = await dominio.CriarAsync(cliente);

            #region Verificação do teste

            Assert.NotNull(retorno);
            Assert.Equal(cliente, retorno);
            _mocker.Verify<IRepositorioDeClientes>(r => r.Adicionar(cliente), Times.Once);
            _mocker.Verify<IRepositorioDeClientes>(r => r.SalvarAlteracoesAsync(), Times.Once);

            #endregion Verificação do teste
        }


    }
}
