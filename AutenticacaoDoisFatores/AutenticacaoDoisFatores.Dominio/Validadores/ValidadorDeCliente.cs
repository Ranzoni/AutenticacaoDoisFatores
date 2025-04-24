using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;

namespace AutenticacaoDoisFatores.Dominio.Validadores
{
    public static class ValidadorDeCliente
    {
        internal static void Validar(this Cliente cliente)
        {
            if (!NomeEhValido(cliente.Nome))
                ExcecoesCliente.NomeInvalido();

            if (!EmailEhValido(cliente.Email))
                ExcecoesCliente.EmailInvalido();

            if (!NomeDominioEhValido(cliente.NomeDominio))
                ExcecoesCliente.NomeDominioInvalido();

            if (!ChaveAcessoEhValida(cliente.ChaveAcesso))
                ExcecoesCliente.ChaveAcessoInvalida();
        }

        public static bool NomeEhValido(string nome)
        {
            return !nome.EstaVazio() && nome.Length >= 3 && nome.Length <= 50;
        }

        public static bool EmailEhValido(string email)
        {
            return !email.EstaVazio() && email.EhEmail() && email.Length <= 256;
        }

        public static bool NomeDominioEhValido(string nomeDominio)
        {
            return !nomeDominio.EstaVazio()
                && nomeDominio.Length >= 3
                && nomeDominio.Length <= 15
                && !nomeDominio.ExistemCaracteresEspeciaisAcentosOuPontuacoes()
                && !nomeDominio.Contains(' ');
        }

        public static bool ChaveAcessoEhValida(string chaveAcesso)
        {
            return !chaveAcesso.EstaVazio()
                && chaveAcesso.Length >= 3
                && chaveAcesso.Length <= 2000;
        }
    }
}
