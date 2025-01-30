using AutenticacaoDoisFatores.Dominio.Construtores;
using AutenticacaoDoisFatores.Servico.Construtores;
using Bogus;

namespace AutenticacaoDoisFatores.Testes.Compartilhados
{
    internal static class ConstrutorDeClientesTeste
    {
        internal static ConstrutorDeCliente RetornarConstrutorDeCliente(Guid? id = null, string? nome = null, string? email = null, string? nomeDominio = null, string? chaveAcesso = null, bool? ativo = null, DateTime? dataCadastro = null, DateTime? dataAlteracao = null)
        {
            var faker = new Faker();

            var construtor = new ConstrutorDeCliente();
            construtor
                .ComId(id ?? Guid.NewGuid())
                .ComNome(nome ?? faker.Company.CompanyName())
                .ComEmail(email ?? faker.Internet.Email())
                .ComNomeDominio(nomeDominio ?? faker.Internet.DomainWord())
                .ComChaveAcesso(chaveAcesso ?? faker.Random.AlphaNumeric(20))
                .ComAtivo(ativo ?? faker.Random.Bool())
                .ComDataCadastro(dataCadastro ?? faker.Date.Past())
                .ComDataAlteracao(dataAlteracao);

            return construtor;
        }

        internal static ConstrutorDeNovoCliente RetornarConstrutorDeNovoCliente(string? nome = null, string? email = null, string? nomeDominio = null)
        {
            var faker = new Faker();

            return new ConstrutorDeNovoCliente()
                .ComNome(nome ?? faker.Company.CompanyName())
                .ComEmail(email ?? faker.Internet.Email())
                .ComNomeDominio(nomeDominio ?? faker.Internet.DomainWord());
        }
    }
}
