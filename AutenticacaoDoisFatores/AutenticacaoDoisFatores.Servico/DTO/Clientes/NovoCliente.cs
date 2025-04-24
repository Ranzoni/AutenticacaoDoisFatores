using AutenticacaoDoisFatores.Dominio.Construtores;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;
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

        public static explicit operator Cliente(NovoCliente novoCliente)
        {
            return new ConstrutorDeCliente()
                .ComNome(novoCliente.Nome)
                .ComEmail(novoCliente.Email)
                .ComNomeDominio(novoCliente.NomeDominio)
                .ComChaveAcesso(novoCliente.ChaveAcesso)
                .ConstruirNovo();
        }

        public static explicit operator Usuario(NovoCliente novoCliente)
        {
            return new ConstrutorDeUsuario()
                .ComNome(novoCliente.Nome)
                .ComNomeUsuario("Administrador")
                .ComEmail(novoCliente.Email)
                .ComSenha(Criptografia.CriptografarComSha512(novoCliente.SenhaAdm))
                .ComEhAdmin(true)
                .ConstruirNovo();
        }
    }
}
