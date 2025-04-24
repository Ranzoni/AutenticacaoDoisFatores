using System.Text;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados
{
    internal static class Segredos
    {
        private const string CaracteresPermitidos = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        internal static string Gerar(int tamanho = 20)
        {
            var randomico = new Random();
            var resultado = new StringBuilder(tamanho);

            for (int i = 0; i < tamanho; i++)
            {
                var indice = randomico.Next(CaracteresPermitidos.Length);
                resultado.Append(CaracteresPermitidos[indice]);
            }

            return resultado.ToString();
        }
    }
}
