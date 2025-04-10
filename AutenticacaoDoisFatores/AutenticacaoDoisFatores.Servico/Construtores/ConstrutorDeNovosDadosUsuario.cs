using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;

namespace AutenticacaoDoisFatores.Servico.Construtores
{
    public class ConstrutorDeNovosDadosUsuario
    {
        private string _nome = "";
        private string _email = "";
        private string _nomeUsuario = "";
        private string _senha = "";
        private TipoDeAutenticacao? _tipoDeAutenticacao;

        public ConstrutorDeNovosDadosUsuario ComNome(string nome)
        {
            _nome = nome;

            return this;
        }

        public ConstrutorDeNovosDadosUsuario ComNomeUsuario(string nomeUsuario)
        {
            _nomeUsuario = nomeUsuario;

            return this;
        }

        public ConstrutorDeNovosDadosUsuario ComEmail(string email)
        {
            _email = email;

            return this;
        }

        public ConstrutorDeNovosDadosUsuario ComSenha(string senha)
        {
            _senha = senha;

            return this;
        }

        public ConstrutorDeNovosDadosUsuario ComTipoDeAutenticacao(TipoDeAutenticacao? tipoDeAutenticacao)
        {
            _tipoDeAutenticacao = tipoDeAutenticacao;

            return this;
        }

        public NovosDadosUsuario Construir()
        {
            var novosDadosUsuario = new NovosDadosUsuario
                (
                    nome: _nome,
                    nomeUsuario: _nomeUsuario,
                    email: _email,
                    senha: _senha,
                    tipoDeAutenticacao: _tipoDeAutenticacao
                );

            return novosDadosUsuario;
        }
    }
}
