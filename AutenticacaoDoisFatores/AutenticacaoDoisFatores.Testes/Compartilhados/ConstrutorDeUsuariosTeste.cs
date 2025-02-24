using AutenticacaoDoisFatores.Dominio.Construtores;
using AutenticacaoDoisFatores.Servico.Construtores;
using Bogus;

namespace AutenticacaoDoisFatores.Testes.Compartilhados
{
    internal static class ConstrutorDeUsuariosTeste
    {
        private static readonly string _senhaParaTeste = "Teste.De.Senha_1";

        internal static ConstrutorDeUsuario RetornarConstrutor(Guid? id = null, string? nome = null, string? nomeUsuario = null, string? email = null, string? senha = null, bool? ativo = null, DateTime? dataUltimoAcesso = null, DateTime? dataCadastro = null, DateTime? dataAlteracao = null)
        {
            var faker = new Faker();

            return new ConstrutorDeUsuario()
                .ComId(id ?? Guid.NewGuid())
                .ComNome(nome ?? faker.Person.FullName)
                .ComNomeUsuario(nomeUsuario ?? faker.Person.UserName)
                .ComEmail(email ?? faker.Person.Email)
                .ComSenha(senha ?? _senhaParaTeste)
                .ComAtivo(ativo ?? faker.Random.Bool())
                .ComDataUltimoAcesso(dataUltimoAcesso)
                .ComDataCadastro(dataCadastro ?? faker.Date.Past())
                .ComDataAlteracao(dataAlteracao);
        }

        internal static ConstrutorDeNovoUsuario RetornarConstrutorDeNovo(string? nome = null, string? nomeUsuario = null, string? email = null, string? senha = null)
        {
            var faker = new Faker();

            return new ConstrutorDeNovoUsuario()
                .ComNome(nome ?? faker.Person.FullName)
                .ComNomeUsuario(nomeUsuario ?? faker.Person.UserName)
                .ComEmail(email ?? faker.Person.Email)
                .ComSenha(senha ?? "T3ste.de.Senh@");
        }
    }
}
