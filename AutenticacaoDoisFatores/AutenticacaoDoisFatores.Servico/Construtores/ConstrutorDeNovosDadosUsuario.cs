using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;
using AutenticacaoDoisFatores.Servico.DTO.Usuarios;

namespace AutenticacaoDoisFatores.Servico.Construtores
{
    public class ConstrutorDeNovosDadosUsuario
    {
        private string _nome = "";
        private string _nomeUsuario = "";
        private long? _celular;
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

        public ConstrutorDeNovosDadosUsuario ComCelular(long? celular)
        {
            _celular = celular;

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
                    celular: _celular,
                    tipoDeAutenticacao: _tipoDeAutenticacao
                );

            return novosDadosUsuario;
        }
    }
}
