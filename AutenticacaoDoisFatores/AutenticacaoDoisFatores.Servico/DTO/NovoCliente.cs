using AutenticacaoDoisFatores.Dominio.Construtores;

namespace AutenticacaoDoisFatores.Servico.DTO
{
    public class NovoCliente
    {
        private readonly string _chaveDescriptografada;

        public string Nome { get; }
        public string Email { get; }
        public string NomeDominio { get; }
        public string ChaveAcesso { get; }

        public NovoCliente(string nome, string email, string nomeDominio)
        {
            Nome = nome;
            Email = email;
            NomeDominio = nomeDominio;
            (ChaveAcesso, _chaveDescriptografada) = GerarChaveAcesso();
        }

        public string ChaveDescriptografada()
        {
            return _chaveDescriptografada;
        }

        private static (string chaveDescriptografada, string chaveCriptograda) GerarChaveAcesso()
        {
            var chaveDescriptografada = Guid.NewGuid().ToString();

            var criptografia = new ConstrutorDeCriptografia().ConstruirCriptografia();
            var chaveCriptografada = criptografia.Criptografar(chaveDescriptografada);

            return (chaveDescriptografada, chaveCriptografada);
        }
    }
}
