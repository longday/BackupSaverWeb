using System;
using System.Text;

namespace WebUI.Extensions
{
    public static class ArrayExtensions
    {
        public static string ToFormatString<T>(this T[] array, char separator = ',')
        {
            StringBuilder strBuilder = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                strBuilder.Append(array[i]?.ToString() + separator);
            }

            return strBuilder.ToString();
        }
    }
}