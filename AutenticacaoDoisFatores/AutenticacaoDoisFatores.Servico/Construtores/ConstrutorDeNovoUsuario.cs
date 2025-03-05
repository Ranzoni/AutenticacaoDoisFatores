using AutenticacaoDoisFatores.Servico.DTO.Usuarios;

namespace AutenticacaoDoisFatores.Servico.Construtores
{
    public class ConstrutorDeNovoUsuario
    {
        private string _nome = "";
        private string _email = "";
        private string _nomeUsuario = "";
        private string _senha = "";

        public ConstrutorDeNovoUsuario ComNome(string nome)
        {
            _nome = nome;

            return this;
        }

        public ConstrutorDeNovoUsuario ComNomeUsuario(string nomeUsuario)
        {
            _nomeUsuario = nomeUsuario;

            return this;
        }

        public ConstrutorDeNovoUsuario ComEmail(string email)
        {
            _email = email;

            return this;
        }

        public ConstrutorDeNovoUsuario ComSenha(string senha)
        {
            _senha = senha;

            return this;
        }

        public NovoUsuario Construir()
        {
            var novoUsuario = new NovoUsuario
                (
                    nome: _nome,
                    nomeUsuario: _nomeUsuario,
                    email: _email,
                    senha: _senha
                );

            return novoUsuario;
        }

        public NovoUsuario ConstruirComPermissoes()
        {
            var novoUsuario = new NovoUsuario
                (
                    nome: _nome,
                    nomeUsuario: _nomeUsuario,
                    email: _email,
                    senha: _senha
                );

            return novoUsuario;
        }
    }
}
