using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Yoeca.Sql
{
    /// <summary>
    /// SQL command that allows inserting rows into tables.
    /// </summary>
    public sealed class InsertInto : ISqlCommand
    {
        private readonly bool m_UpdateOnDuplicateKey;

        [NotNull]
        public readonly string Table;

        [NotNull]
        public readonly ImmutableList<KeyValuePair<string, string>> Values;

        public InsertInto(
            [NotNull] string table,
            [NotNull] ImmutableList<KeyValuePair<string, string>> values,
            bool updateOnDuplicatKey)
        {
            Table = table;
            Values = values;
            m_UpdateOnDuplicateKey = updateOnDuplicatKey;
        }

        /// <summary>
        /// The SQL command that attempts an UPSERT operation when a row with the same primary key.
        /// </summary>
        [NotNull]
        public InsertInto UpdateOnDuplicateKey => new InsertInto(Table, Values, true);

        public string Format(SqlFormat format)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("INSERT INTO {0} ({1}) ", Table, string.Join(", ", Values.Select(x => x.Key)));
            builder.AppendFormat("VALUES ({0})", string.Join(", ", Values.Select(x => x.Value)));

            if (m_UpdateOnDuplicateKey)
            {
                builder.AppendLine();
                builder.AppendFormat("ON DUPLICATE KEY UPDATE {0}",
                    string.Join(", ", Values.Select(x => x.Key + "=" + x.Value)));
            }

            return builder.ToString();
        }

        [NotNull]
        public static InsertInto Row<TRecord>([NotNull] TRecord record)
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

            return new InsertInto(definition.Name, parameters.ToImmutableList(), false);
        }
    }
}