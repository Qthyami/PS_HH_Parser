using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PSHhParser
{
    public class StringToNumberParser
    {
        public static class ParserHelper
        {
            public static T ParseNumber<T>(string input, string errorContext = "") where T : struct, IConvertible
            {
                if (typeof(T) == typeof(int))
                {
                    if (int.TryParse(input, out int result))
                        return (T)Convert.ChangeType(result, typeof(T));
                }
                else if (typeof(T) == typeof(long))
                {
                    if (long.TryParse(input, out long result))
                        return (T)Convert.ChangeType(result, typeof(T));
                }
                else if (typeof(T) == typeof(decimal))
                {
                    if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result))
                        return (T)Convert.ChangeType(result, typeof(T));
                }
                else if (typeof(T) == typeof(double))
                {
                    if (double.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out double result))
                        return (T)Convert.ChangeType(result, typeof(T));
                }
                else
                {
                    throw new NotSupportedException($"Type {typeof(T).Name} is not supported in ParseNumber.");
                }

                throw new FormatException($"Error parsing '{input}'{(string.IsNullOrEmpty(errorContext) ? "" : $" for {errorContext}")}.");
            }
        }

    }
}
