using AutenticacaoDoisFatores.Dominio.Construtores;
using AutenticacaoDoisFatores.Servico.Construtores;
using Bogus;

namespace AutenticacaoDoisFatores.Testes.Compartilhados
{
    internal static class ConstrutorDeClientesTeste
    {
        internal static ConstrutorDeCliente RetornarConstrutor(Guid? id = null, string? nome = null, string? email = null, string? nomeDominio = null, string? chaveAcesso = null, bool? ativo = null, DateTime? dataCadastro = null, DateTime? dataAlteracao = null)
        {
            var faker = new Faker();

            var construtor = new ConstrutorDeCliente();
            construtor
                .ComId(id ?? Guid.NewGuid())
                .ComNome(nome ?? faker.Company.CompanyName())
                .ComEmail(email ?? faker.Internet.Email())
                .ComNomeDominio(nomeDominio ?? "dominio_cliente")
                .ComChaveAcesso(chaveAcesso ?? faker.Random.AlphaNumeric(20))
                .ComAtivo(ativo ?? faker.Random.Bool())
                .ComDataCadastro(dataCadastro ?? faker.Date.Past())
                .ComDataAlteracao(dataAlteracao);

            return construtor;
        }

        internal static ConstrutorDeNovoCliente RetornarConstrutorDeNovo(string? nome = null, string? email = null, string? nomeDominio = null, string? senhaAdm = null)
        {
            var faker = new Faker();

            return new ConstrutorDeNovoCliente()
                .ComNome(nome ?? faker.Company.CompanyName())
                .ComEmail(email ?? faker.Internet.Email())
                .ComNomeDominio(nomeDominio ?? "dominio_cliente")
                .ComSenhaAdm(senhaAdm ?? "T3ste.de.Senh@");
        }
    }
}
