using AutenticacaoDoisFatores.Servico.Compartilhados;

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
            (_chaveDescriptografada, ChaveAcesso) = GerarChaveAcesso();
        }

        public string ChaveDescriptografada()
        {
            return _chaveDescriptografada;
        }

        private static (string chaveDescriptografada, string chaveCriptograda) GerarChaveAcesso()
        {
            return Seguranca.GerarChaveComCriptografia();
        }
    }
}
