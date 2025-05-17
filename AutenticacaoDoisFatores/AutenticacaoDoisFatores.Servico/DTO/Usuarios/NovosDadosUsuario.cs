using AutenticacaoDoisFatores.Dominio.Compartilhados.Usuarios;

namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class NovosDadosUsuario(string nome, string nomeUsuario, long? celular, TipoDeAutenticacao? tipoDeAutenticacao)
    {
        public string Nome { get; } = nome;
        public string NomeUsuario { get; } = nomeUsuario;
        public long? Celular { get; } = celular;
        public TipoDeAutenticacao? TipoDeAutenticacao { get; } = tipoDeAutenticacao;
    }
}
