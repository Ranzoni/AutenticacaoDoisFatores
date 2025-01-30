using AutenticacaoDoisFatores.Dominio.Entidades;

namespace AutenticacaoDoisFatores.Dominio.Construtores
{
    public class ConstrutorDeUsuario
    {
        private Guid _id;
        private string _nome = "";
        private string _nomeUsuario = "";
        private string _email = "";
        private string _senha = "";
        private bool _ativo;
        private DateTime? _dataUltimoAcesso;
        private DateTime _dataCadastro;
        private DateTime? _dataAlteracao;

        public ConstrutorDeUsuario ComNome(string nome)
        {
            _nome = nome;

            return this;
        }

        public ConstrutorDeUsuario ComNomeUsuario(string nomeUsuario)
        {
            _nomeUsuario = nomeUsuario;

            return this;
        }

        public ConstrutorDeUsuario ComEmail(string email)
        {
            _email = email;

            return this;
        }

        public ConstrutorDeUsuario ComSenha(string senha)
        {
            _senha = senha;

            return this;
        }

        public Usuario ConstruirNovo()
        {
            var usuario = new Usuario(nome: _nome, nomeUsuario: _nomeUsuario, email: _email, senha: _senha);

            return usuario;
        }
    }
}
