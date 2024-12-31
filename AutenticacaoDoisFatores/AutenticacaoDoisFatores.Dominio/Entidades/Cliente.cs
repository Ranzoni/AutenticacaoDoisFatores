using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades;
using AutenticacaoDoisFatores.Dominio.Validadores;

namespace AutenticacaoDoisFatores.Dominio.Entidades
{
    public class Cliente : EntidadeComAuditoria
    {
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string NomeDominio { get; private set; }
        public Guid ChaveAcesso { get; private set; } = Guid.NewGuid();
        public bool Ativo { get; private set; } = false;

        public Cliente(string nome, string email)
        {
            Nome = nome;
            Email = email;
            NomeDominio = Nome.Split(" ").First().RemoverCaracteresEspeciais().ToLower();

            this.ValidarCriacao();
        }

        public Cliente(Guid id, string nome, string email, string nomeDominio, Guid chaveAcesso, bool ativo, DateTime dataCadastro, DateTime? dataAlteracao)
        {
            Id = id;
            Nome = nome;
            Email = email;
            NomeDominio = nomeDominio;
            ChaveAcesso = chaveAcesso;
            Ativo = ativo;
            DataCadastro = dataCadastro;
            DataAlteracao = dataAlteracao;
        }
    }
}
