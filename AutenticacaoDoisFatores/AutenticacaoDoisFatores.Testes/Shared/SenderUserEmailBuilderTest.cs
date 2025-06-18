using AutenticacaoDoisFatores.Service.Builders;
using Bogus;

namespace AutenticacaoDoisFatores.Tests.Shared
{
    internal static class SenderUserEmailBuilderTest
    {
        internal static SenderUserEmailBuilder GetBuilder(string subject = "", string message = "", string htmlEmail = "")
        {
            var faker = new Faker();

            return new SenderUserEmailBuilder()
                .WithSubject(subject ?? faker.Lorem.Word())
                .WithMessage(message ?? faker.Lorem.Sentence())
                .WithHtmlEmail(htmlEmail ?? faker.Lorem.Sentence());
        }
    }
}
