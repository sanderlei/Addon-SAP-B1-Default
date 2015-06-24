using System;
using System.Text.RegularExpressions;

namespace Common.Util
{
    public class FormatString
    {
        public static string RemoveNonNumericChars(string value, string decimalSeparator)
        {
            Regex regex = new Regex(String.Format(@"[^-?\d+\{0}]", decimalSeparator));
            return regex.Replace(value, "");
        }
    }
}
