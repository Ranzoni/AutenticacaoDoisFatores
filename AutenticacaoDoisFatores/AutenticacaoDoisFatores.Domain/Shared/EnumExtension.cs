using System.ComponentModel;

namespace AutenticacaoDoisFatores.Domain.Shared
{
    public static class EnumExtension
    {
        public static string? Description<T>(this T value) where T : struct, IConvertible
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
