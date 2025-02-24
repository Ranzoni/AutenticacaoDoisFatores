using AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades;
using AutenticacaoDoisFatores.Dominio.Validadores;

namespace AutenticacaoDoisFatores.Dominio.Entidades
{
    public class Usuario : EntidadeComAuditoria
    {
        public string Nome { get; private set; } = "";
        public string NomeUsuario { get; private set; } = "";
        public string Email { get; private set; } = "";
        public string Senha { get; private set; } = "";
        public bool Ativo { get; private set; }
        public DateTime? DataUltimoAcesso { get; private set; }

        public Usuario(string nome, string nomeUsuario, string email, string senha)
        {
            Nome = nome;
            NomeUsuario = nomeUsuario;
            Email = email;
            Senha = senha;

            this.Validar();
        }

        public Usuario(Guid id, string nome, string nomeUsuario, string email, string senha, bool ativo, DateTime? dataUltimoAcesso, DateTime dataCadastro, DateTime? dataAlteracao)
            : base(true)
        {
            Id = id;
            Nome = nome;
            NomeUsuario = nomeUsuario;
            Email = email;
            Senha = senha;
            Ativo = ativo;
            DataUltimoAcesso = dataUltimoAcesso;
            DataCadastro = dataCadastro;
            DataAlteracao = dataAlteracao;

            this.Validar();
        }

        public void Ativar(bool valor)
        {
            Ativo = valor;

            AtualizarDataAlteracao();
        }
    }
}
