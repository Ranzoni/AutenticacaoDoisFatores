using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Entidades;
using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
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
        public bool EhAdmin { get; private set; }
        public TipoDeAutenticacao? TipoDeAutenticacao { get; private set; }

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

        public Usuario(Guid id, string nome, string nomeUsuario, string email, string senha, bool ativo, DateTime? dataUltimoAcesso, DateTime dataCadastro, DateTime? dataAlteracao, bool ehAdmin, TipoDeAutenticacao? tipoDeAutenticacao)
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
            TipoDeAutenticacao = tipoDeAutenticacao;

            this.Validar();
        }

        public void AlterarNome(string nome)
        {
            AuditarModificacao("Nome", Nome, nome);

            Nome = nome;

            this.Validar();
        }

        public void AlterarNomeUsuario(string nomeUsuario)
        {
            AuditarModificacao("NomeUsuario", NomeUsuario, nomeUsuario);

            NomeUsuario = nomeUsuario;

            this.Validar();
        }

        public void AlterarEmail(string email)
        {
            AuditarModificacao("Email", Email, email);

            Email = email;

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

            this.Validar();
        }

        public void AtualizarDataUltimoAcesso()
        {
            var novaData = DateTime.Now;

            AuditarModificacao("DataUltimoAcesso", $"{DataUltimoAcesso:yyyy-MM-dd HH:mm:ss}", $"{novaData:yyyy-MM-dd HH:mm:ss}");

            DataUltimoAcesso = novaData;
        }

        public bool ExisteTipoDeAutenticacaoConfigurado()
        {
            return TipoDeAutenticacao is not null;
        }

        public void ConfigurarTipoDeAutenticacao(TipoDeAutenticacao? tipoDeAutenticacao)
        {
            AuditarModificacao("AutenticacaoDoisFatores", TipoDeAutenticacao.ToString()!, tipoDeAutenticacao.ToString()!);

            TipoDeAutenticacao = tipoDeAutenticacao;
        }
    }
}
