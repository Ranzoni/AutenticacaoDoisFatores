using AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades;
using AutenticacaoDoisFatores.Dominio.Validadores;
using System.Drawing;

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
        public bool EhAdmin { get; private set; }

        public Usuario(string nome, string nomeUsuario, string email, string senha, bool ehAdmin = false)
        {
            Nome = nome;
            NomeUsuario = nomeUsuario;
            Email = email;
            Senha = senha;

            if (ehAdmin)
            {
                Ativo = true;
                EhAdmin = true;
            }

            this.Validar();
        }

        public Usuario(Guid id, string nome, string nomeUsuario, string email, string senha, bool ativo, DateTime? dataUltimoAcesso, DateTime dataCadastro, DateTime? dataAlteracao, bool ehAdmin)
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
            EhAdmin = ehAdmin;

            this.Validar();
        }

        public void Ativar(bool valor)
        {
            AuditarModificacao("Ativo", Ativo.ToString(), valor.ToString());

            Ativo = valor;
        }

        public void AlterarSenha(string senha)
        {
            AuditarModificacao("Senha", Senha, senha);

            Senha = senha;
        }

        public void AtualizarDataUltimoAcesso()
        {
            var novaData = DateTime.Now;

            AuditarModificacao("DataUltimoAcesso", $"{DataUltimoAcesso:yyyy-MM-dd HH:mm:ss}", $"{novaData:yyyy-MM-dd HH:mm:ss}");

            DataUltimoAcesso = novaData;
        }
    }
}
