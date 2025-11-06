using System;
using System.Collections.Generic;
using System.Linq;

namespace Yoeca.Sql
{
    internal static class SqlIdentifier
    {
        public static string Quote(string identifier, SqlFormat format)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("Identifier cannot be null or whitespace.", nameof(identifier));
            }

            return format switch
            {
                SqlFormat.MySql => QuoteMySql(identifier),
                _ => throw new NotSupportedException("Unsupported SQL format: " + format),
            };
        }

        public static IEnumerable<string> Quote(IEnumerable<string> identifiers, SqlFormat format)
        {
            return identifiers.Select(identifier => Quote(identifier, format));
        }

        private static string QuoteMySql(string identifier)
        {
            static string QuoteSegment(string value)
            {
                string escaped = value.Replace("`", "``", StringComparison.Ordinal);
                return $"`{escaped}`";
            }

            if (identifier.IndexOf('.') == -1)
            {
                return QuoteSegment(identifier);
            }

            var segments = identifier.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return string.Join(".", segments.Select(QuoteSegment));
        }
    }
}
