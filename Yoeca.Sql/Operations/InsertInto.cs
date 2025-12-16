using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Globalization;
using System.Text;

namespace Yoeca.Sql
{
    /// <summary>
    /// Generic insert-into operation that will get the latest insert ID for auto-increment columns.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class InsertInto<T> : ISqlCommand<T>
        where T : struct
    {
        private readonly InsertInto m_Source;
        private static readonly Type s_TargetType = typeof(T);

        internal InsertInto(InsertInto source)
        {
            m_Source = source;
        }

        public T TranslateRow(ISqlFields fields)
        {
            var value = fields.Get(0);

            if (value == null)
            {
                return default(T);
            }

            if (value is T expectedValue)
            {
                return expectedValue;
            }

            try
            {
                return (T)Convert.ChangeType(value, s_TargetType, CultureInfo.InvariantCulture);
            }
            catch
            {
                return default(T);
            }
        }

        public SqlCommandText Format(SqlFormat format)
        {
            return m_Source.Format(format);
        }
    }

    /// <summary>
    /// SQL command that allows inserting rows into tables.
    /// </summary>
    public sealed class InsertInto : ISqlCommand
    {
        private readonly bool m_UpdateOnDuplicateKey;

        private readonly bool m_GetLastInsertIdentity;

        private readonly DataType m_AutoIncrementType;

        public readonly string Table;

        public readonly ImmutableList<KeyValuePair<string, string>> Values;


        internal InsertInto(
            string table,
            ImmutableList<KeyValuePair<string, string>> values,
            bool updateOnDuplicateKey, bool getLastInsertIdentity,
            DataType autoIncrementType)
        {
            Table = table;
            Values = values;
            m_UpdateOnDuplicateKey = updateOnDuplicateKey;
            m_GetLastInsertIdentity = getLastInsertIdentity;
            m_AutoIncrementType = autoIncrementType;
        }

        /// <summary>
        /// The SQL command that attempts an UPSERT operation when a row with the same primary key.
        /// </summary>
        public InsertInto UpdateOnDuplicateKey =>
            new InsertInto(Table, Values, true, m_GetLastInsertIdentity, m_AutoIncrementType);

        /// <summary>
        /// The SQL command will return the last insert identity for the auto incremented field of the table.
        /// </summary>
        public InsertInto<T> GetLastInsertIdentity<T>()
            where T : struct
        {
            if (m_AutoIncrementType == DataType.Unknown)
            {
                throw new InvalidOperationException("There is no matching auto-increment identity.");
            }

            var value = new InsertInto(Table, Values, m_UpdateOnDuplicateKey, true, m_AutoIncrementType);

            return new InsertInto<T>(value);
        }

        public SqlCommandText Format(SqlFormat format)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("INSERT INTO {0} ({1}) ",
                                 SqlIdentifier.Quote(Table, format),
                                 string.Join(", ", SqlIdentifier.Quote(Values.Select(x => x.Key), format)));
            builder.AppendFormat("VALUES ({0})", string.Join(", ", Values.Select(x => x.Value)));

            if (m_UpdateOnDuplicateKey)
            {
                builder.AppendLine();
                builder.AppendFormat("ON DUPLICATE KEY UPDATE {0}",
                                     string.Join(", ",
                                                 Values.Select(x =>
                                                     SqlIdentifier.Quote(x.Key, format) + "=" + x.Value)));
            }

            if (m_GetLastInsertIdentity)
            {
                builder.AppendLine(";");
                builder.Append("SELECT LAST_INSERT_ID();");
            }

            return SqlCommandText.WithoutParameters(builder.ToString());
        }

        public static InsertInto Row<TRecord>(TRecord record)
            where TRecord: notnull
        {
            var definition = new TableDefinition(typeof(TRecord));

            var parameters = new List<KeyValuePair<string, string>>();
            DataType autoIncrementType = DataType.Unknown;
            foreach (var columnRetriever in definition.Columns)
            {
                // Skip auto-incrementing values.
                if (columnRetriever.TableColumn.AutoIncrement)
                {
                    autoIncrementType = columnRetriever.TableColumn.DataType;
                    continue;
                }

                var key = columnRetriever.Name;
                var value = columnRetriever.Get(record);

                if (value is null)
                {
                    if (columnRetriever.TableColumn.NotNull)
                    {
                        throw new InvalidOperationException("Value cannot be converted.");
                    }

                    value = "NULL";
                }

                if (columnRetriever.RequiresEscaping && !string.Equals(value, "NULL", StringComparison.OrdinalIgnoreCase))
                {
                    value = "'" + value + "'";
                }

                parameters.Add(new KeyValuePair<string, string>(key, value));
            }

            return new InsertInto(definition.Name, parameters.ToImmutableList(), false, false, autoIncrementType);
        }
    }
}
