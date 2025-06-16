using AutenticacaoDoisFatores.Servico.Construtores;
using Bogus;

namespace AutenticacaoDoisFatores.Testes.Compartilhados
{
    internal static class ConstrutorDeEnvioEmailParaUsuarioTeste
    {
        internal static SenderUserEmailBuilder RetornarConstrutor(string titulo = "", string mensagem = "", string htmlEmail = "")
        {
            var faker = new Faker();

            return new SenderUserEmailBuilder()
                .WithSubject(titulo ?? faker.Lorem.Word())
                .WithMessage(mensagem ?? faker.Lorem.Sentence())
                .WithHtmlEmail(htmlEmail ?? faker.Lorem.Sentence());
        }
    }
}
