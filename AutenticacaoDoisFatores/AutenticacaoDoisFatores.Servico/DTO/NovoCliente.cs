namespace AutenticacaoDoisFatores.Servico.DTO
{
    public class NovoCliente(string nome, string email)
    {
        public string Nome { get; } = nome;
        public string Email { get; } = email;
    }
}
