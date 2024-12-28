using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;

namespace AutenticacaoDoisFatores.Dominio.Validadores
{
    public static class ValidadorDeCliente
    {
        internal static void ValidarCriacao(this Cliente cliente)
        {
            if (!NomeEhValido(cliente.Nome))
                ExcecoesCliente.NomeNaoPreenchido();

            if (!EmailEhValido(cliente.Email))
                ExcecoesCliente.EmailNaoPreenchido();
        }

        public static bool NomeEhValido(string nome)
        {
            return !nome.EstaVazio() && nome.Length >= 3 && nome.Length <= 50;
        }

        public static bool EmailEhValido(string email)
        {
            return !email.EstaVazio() && email.Length >= 5 && email.Length <= 256;
        }
    }
}
