using AutenticacaoDoisFatores.Servico.Compartilhados;

namespace AutenticacaoDoisFatores.Servico.DTO.Clientes
{
    public class NovoCliente
    {
        private readonly string _chaveDescriptografada;

        public string Nome { get; }
        public string Email { get; }
        public string NomeDominio { get; }
        public string ChaveAcesso { get; }
        public string SenhaAdm { get; }

        public NovoCliente(string nome, string email, string nomeDominio, string senhaAdm)
        {
            Nome = nome;
            Email = email;
            NomeDominio = nomeDominio;
            (_chaveDescriptografada, ChaveAcesso) = GerarChaveAcesso();
            SenhaAdm = senhaAdm;
        }

        public string ChaveDescriptografada()
        {
            return _chaveDescriptografada;
        }

        private static (string chaveDescriptografada, string chaveCriptograda) GerarChaveAcesso()
        {
            return Seguranca.GerarChaveDeAcessoComCriptografia();
        }
    }
}
