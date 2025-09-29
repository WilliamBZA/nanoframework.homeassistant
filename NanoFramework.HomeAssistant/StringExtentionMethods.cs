using System;
using System.Text;

namespace NanoFramework.HomeAssistant
{
    public static class StringExtentionMethods
    {
        public static string Replace(this string source, string oldValue, string newValue)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (oldValue == null) throw new ArgumentNullException(nameof(oldValue));
            if (oldValue.Length == 0) throw new ArgumentException("oldValue cannot be empty.", nameof(oldValue));

            if (newValue == null) newValue = string.Empty;

            var result = new StringBuilder();
            int startIndex = 0;
            int index;

            while ((index = source.IndexOf(oldValue, startIndex)) != -1)
            {
                result.Append(source, startIndex, index - startIndex);
                result.Append(newValue);

                startIndex = index + oldValue.Length;
            }

            result.Append(source, startIndex, source.Length - startIndex);

            return result.ToString();
        }

        public static string Join(string separator, string[] values)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;

            foreach (var value in values)
            {
                if (!first)
                {
                    sb.Append(separator);
                }
                sb.Append(value);
                first = false;
            }

            return sb.ToString();
        }
    }
}