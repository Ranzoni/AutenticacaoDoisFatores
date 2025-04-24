namespace AutenticacaoDoisFatores.Servico.DTO.Usuarios
{
    public class TrocarEmailUsuario(string email)
    {
        public string Email { get; } = email;

        public static explicit operator NovosDadosUsuario(TrocarEmailUsuario trocarEmailUsuario)
        {
            return new NovosDadosUsuario(nome: null, nomeUsuario: null, email: trocarEmailUsuario.Email, senha: null, celular: null, tipoDeAutenticacao: null);
        }
    }
}
