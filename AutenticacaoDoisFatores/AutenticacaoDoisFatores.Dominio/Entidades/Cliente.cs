using AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades;
using AutenticacaoDoisFatores.Dominio.Validadores;

namespace AutenticacaoDoisFatores.Dominio.Entidades
{
    public class Cliente : EntidadeComAuditoria
    {
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string NomeDominio { get; private set; }
        public string ChaveAcesso { get; private set; }
        public bool Ativo { get; private set; } = false;

        public Cliente(string nome, string email, string nomeDominio, string chaveAcesso)
        {
            Nome = nome;
            Email = email;
            NomeDominio = nomeDominio;
            ChaveAcesso = chaveAcesso;

            this.ValidarCriacao();
        }

        public Cliente(Guid id, string nome, string email, string nomeDominio, string chaveAcesso, bool ativo, DateTime dataCadastro, DateTime? dataAlteracao)
            : base(true)
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

        public void Ativar(bool valor)
        {
            Ativo = valor;

            AtualizarDataAlteracao();
        }
    }
}
