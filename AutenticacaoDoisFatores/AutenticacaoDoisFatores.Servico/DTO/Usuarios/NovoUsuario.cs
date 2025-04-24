using AutenticacaoDoisFatores.Dominio.Construtores;
using AutenticacaoDoisFatores.Dominio.Dominios;
using AutenticacaoDoisFatores.Dominio.Entidades;

namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class NovoUsuario(string nome, string nomeUsuario, string email, string senha, long? celular)
    {
        private readonly string _senhaDescriptografada = senha;

        public string Nome { get; } = nome;
        public string NomeUsuario { get; } = nomeUsuario;
        public string Email { get; } = email;
        public string Senha { get; } = Criptografia.CriptografarComSha512(senha);
        public long? Celular { get; } = celular;

        public string SenhaDescriptografada()
        {
            return _senhaDescriptografada;
        }

        public static explicit operator Usuario(NovoUsuario novoUsuario)
        {
            return new ConstrutorDeUsuario()
                .ComNome(novoUsuario.Nome)
                .ComNomeUsuario(novoUsuario.NomeUsuario)
                .ComEmail(novoUsuario.Email)
                .ComSenha(novoUsuario.Senha)
                .ComCelular(novoUsuario.Celular)
                .ConstruirNovo();
        }
    }
}
