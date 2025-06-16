using AutenticacaoDoisFatores.Dominio.Construtores;
using AutenticacaoDoisFatores.Servico.Construtores;
using Bogus;

namespace AutenticacaoDoisFatores.Testes.Compartilhados
{
    internal static class ConstrutorDeClientesTeste
    {
        internal static ClientBuilder RetornarConstrutor(Guid? id = null, string? nome = null, string? email = null, string? nomeDominio = null, string? chaveAcesso = null, bool? ativo = null, DateTime? dataCadastro = null, DateTime? dataAlteracao = null)
        {
            var faker = new Faker();

            var construtor = new ClientBuilder();
            construtor
                .WithId(id ?? Guid.NewGuid())
                .WithName(nome ?? faker.Company.CompanyName())
                .WithEmail(email ?? faker.Internet.Email())
                .WithDomainName(nomeDominio ?? "dominio_cliente")
                .WithAccessKey(chaveAcesso ?? faker.Random.AlphaNumeric(20))
                .WithActive(ativo ?? faker.Random.Bool())
                .WithCreatedAt(dataCadastro ?? faker.Date.Past())
                .WithUpdatedAt(dataAlteracao);

            return construtor;
        }

        internal static NewClientBuilder RetornarConstrutorDeNovo(string? nome = null, string? email = null, string? nomeDominio = null, string? senhaAdm = null)
        {
            var faker = new Faker();

            return new NewClientBuilder()
                .WithName(nome ?? faker.Company.CompanyName())
                .WithEmail(email ?? faker.Internet.Email())
                .WithDomainName(nomeDominio ?? "dominio_cliente")
                .WithAdminPassword(senhaAdm ?? "T3ste.de.Senh@");
        }
    }
}
