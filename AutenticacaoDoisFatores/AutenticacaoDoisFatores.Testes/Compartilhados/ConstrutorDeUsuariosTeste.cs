using AutenticacaoDoisFatores.Dominio.Construtores;
using AutenticacaoDoisFatores.Servico.Construtores;
using Bogus;

namespace AutenticacaoDoisFatores.Testes.Compartilhados
{
    internal static class ConstrutorDeUsuariosTeste
    {
        private static readonly string _senhaParaTeste = "Teste.De.Senha_1";

        internal static ConstrutorDeUsuario RetornarConstrutor(Guid? id = null, string? nome = null, string? nomeUsuario = null, string? email = null, string? senha = null, bool? ativo = null, DateTime? dataUltimoAcesso = null, DateTime? dataCadastro = null, DateTime? dataAlteracao = null, bool? ehAdm = null)
        {
            var faker = new Faker();

            return new ConstrutorDeUsuario()
                .ComId(id ?? Guid.NewGuid())
                .ComNome(nome ?? faker.Person.FullName)
                .ComNomeUsuario(nomeUsuario ?? "teste_user_2010234")
                .ComEmail(email ?? faker.Person.Email)
                .ComSenha(senha ?? _senhaParaTeste)
                .ComAtivo(ativo ?? faker.Random.Bool())
                .ComDataUltimoAcesso(dataUltimoAcesso)
                .ComDataCadastro(dataCadastro ?? faker.Date.Past())
                .ComDataAlteracao(dataAlteracao)
                .ComEhAdmin(ehAdm ?? false);
        }

        internal static ConstrutorDeNovoUsuario RetornarConstrutorDeNovo(string? nome = null, string? nomeUsuario = null, string? email = null, string? senha = null)
        {
            var faker = new Faker();

            return new ConstrutorDeNovoUsuario()
                .ComNome(nome ?? faker.Person.FullName)
                .ComNomeUsuario(nomeUsuario ?? "teste_user_2010234")
                .ComEmail(email ?? faker.Person.Email)
                .ComSenha(senha ?? "T3ste.de.Senh@");
        }

        internal static ConstrutorDeNovosDadosUsuario RetornarConstrutorDeNovosDados(string? nome = null, string? nomeUsuario = null, string? email = null, string? senha = null)
        {
            return new ConstrutorDeNovosDadosUsuario()
                .ComNome(nome)
                .ComNomeUsuario(nomeUsuario)
                .ComEmail(email)
                .ComSenha(senha);
        }
    }
}
