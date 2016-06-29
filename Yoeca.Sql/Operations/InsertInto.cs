using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Yoeca.Sql
{
    public sealed class InsertInto : ISqlCommand
    {
        public readonly string Table;
        public readonly ImmutableList<KeyValuePair<string, string>> Values;

        public InsertInto([NotNull] string table, [NotNull] ImmutableList<KeyValuePair<string, string>> values)
        {
            Table = table;
            Values = values;
        }

        public string Format(SqlFormat format)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("INSERT INTO {0} ({1}) ", Table, string.Join(", ", Values.Select(x => x.Key)));
            builder.AppendFormat("VALUES ({0})", string.Join(", ", Values.Select(x => x.Value)));

            return builder.ToString();
        }

        [NotNull]
        public static InsertInto Row<TRecord>(TRecord record)
        {
            var definition = new TableDefinition(typeof(TRecord));

            var parameters = new List<KeyValuePair<string, string>>();
            foreach (var columnRetriever in definition.Columns)
            {
                var key = columnRetriever.Name;
                var value = columnRetriever.Get(record);

                if (columnRetriever.RequiresEscaping)
                {
                    value = "'" + value + "'";
                }

                parameters.Add(new KeyValuePair<string, string>(key, value));
            }

            return new InsertInto(definition.Name, parameters.ToImmutableList());
        }
    }
}