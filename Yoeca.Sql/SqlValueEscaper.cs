using System;

namespace Yoeca.Sql
{
    internal static class SqlValueEscaper
    {
        public static string Escape(string value)
        {
            var result = value.Replace("\\", "\\\\", StringComparison.Ordinal);
            result = result.Replace("'", "''", StringComparison.Ordinal);
            return result;
        }
    }
}
