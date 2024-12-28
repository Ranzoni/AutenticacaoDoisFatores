using System.ComponentModel;

namespace AutenticacaoDoisFatores.Dominio.Compartilhados
{
    public static class ExtensaoEnum
    {
        public static string? Descricao<T>(this T value) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return null;

            var descricao = value.ToString();
            var fieldInfo = value.GetType().GetField(value.ToString() ?? "");

            if (fieldInfo != null)
            {
                var atributos = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (atributos != null && atributos.Length > 0)
                    descricao = ((DescriptionAttribute)atributos[0]).Description;
            }

            return descricao;
        }
    }
}
