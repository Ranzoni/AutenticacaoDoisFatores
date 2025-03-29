using AutenticacaoDoisFatores.Servico.DTO.Usuarios;

namespace AutenticacaoDoisFatores.Servico.Construtores
{
    public class ConstrutorDeNovosDadosUsuario
    {
        private string? _nome;
        private string? _nomeUsuario;

        public ConstrutorDeNovosDadosUsuario ComNome(string? nome)
        {
            _nome = nome;

            return this;
        }

        public ConstrutorDeNovosDadosUsuario ComNomeUsuario(string? nomeUsuario)
        {
            _nomeUsuario = nomeUsuario;

            return this;
        }

        public NovosDadosUsuario Construir()
        {
            return new NovosDadosUsuario
                (
                    nome: _nome,
                    nomeUsuario: _nomeUsuario
                );
        }
    }
}
