using AutenticacaoDoisFatores.Dominio.Compartilhados;
using AutenticacaoDoisFatores.Dominio.Entidades;
using AutenticacaoDoisFatores.Dominio.Excecoes;

namespace AutenticacaoDoisFatores.Dominio.Validadores
{
    public static class ValidadorDeUsuario
    {
        internal static void Validar(this Usuario usuario)
        {
            if (!NomeEhValido(usuario.Nome))
                ExcecoesUsuario.NomeInvalido();

            if (!NomeUsuarioEhValido(usuario.NomeUsuario))
                ExcecoesUsuario.NomeUsuarioInvalido();

            if (!EmailEhValido(usuario.Email))
                ExcecoesUsuario.EmailInvalido();

            if (!SenhaEhValida(usuario.Senha))
                ExcecoesUsuario.SenhaInvalida();

            if (!CelularEhValido(usuario.Celular))
                ExcecoesUsuario.CelularInvalido();

            if (!ChaveSecretaEhValida(usuario.ChaveSecreta))
                ExcecoesUsuario.ChaveSecretaInvalida();
        }

        public static bool NomeEhValido(string nome)
        {
            return !nome.EstaVazio() && nome.Length >= 3 && nome.Length <= 50;
        }

        public static bool NomeUsuarioEhValido(string nomeUsuario)
        {
            return !nomeUsuario.EstaVazio() && nomeUsuario.Length >= 5 && nomeUsuario.Length <= 20;
        }

        public static bool EmailEhValido(string email)
        {
            return !email.EstaVazio() && email.EhEmail() && email.Length <= 256;
        }

        public static bool SenhaEhValida(string senha)
        {
            return !senha.EstaVazio() && senha.Length <= 256;
        }

        public static bool CelularEhValido(long? celular)
        {
            return celular is null || celular > 99999;
        }

        public static bool ChaveSecretaEhValida(string chaveSecreta)
        {
            return !chaveSecreta.EstaVazio() && chaveSecreta.Length == 20;
        }
    }
}
