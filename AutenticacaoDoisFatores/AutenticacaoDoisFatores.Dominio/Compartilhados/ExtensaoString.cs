namespace AutenticacaoDoisFatores.Dominio.Compartilhados
{
    public static class ExtensaoString
    {
        public static bool EstaVazio(this string valor)
        {
            return string.IsNullOrEmpty(valor) || string.IsNullOrWhiteSpace(valor);
        }
    }
}
