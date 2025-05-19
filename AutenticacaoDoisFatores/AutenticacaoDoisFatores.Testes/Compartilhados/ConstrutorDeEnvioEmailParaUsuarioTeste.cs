using AutenticacaoDoisFatores.Servico.Construtores;
using Bogus;

namespace AutenticacaoDoisFatores.Testes.Compartilhados
{
    internal static class ConstrutorDeEnvioEmailParaUsuarioTeste
    {
        internal static ConstrutorDeEnvioEmailParaUsuario RetornarConstrutor(string titulo = "", string mensagem = "", string htmlEmail = "")
        {
            var faker = new Faker();

            return new ConstrutorDeEnvioEmailParaUsuario()
                .ComTitulo(titulo ?? faker.Lorem.Word())
                .ComMensagem(mensagem ?? faker.Lorem.Sentence())
                .ComHtmlEmail(htmlEmail ?? faker.Lorem.Sentence());
        }
    }
}
