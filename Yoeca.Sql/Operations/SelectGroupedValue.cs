using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;

namespace Yoeca.Sql
{
    /// <summary>
    /// Represents a SELECT statement that returns grouped sum values.
    /// </summary>
    /// <typeparam name="TTable">The entity that maps to the database table.</typeparam>
    /// <typeparam name="TGroup">Type of the group column.</typeparam>
    /// <typeparam name="TValue">Type of the aggregated column.</typeparam>
    public sealed class SelectGroupedValue<TTable, TGroup, TValue> : ISqlCommand<GroupedValue<TGroup, TValue>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectGroupedValue{TTable, TGroup, TValue}"/> class.
        /// </summary>
        /// <param name="table">Name of the table.</param>
        /// <param name="groupColumn">Column that is used for the GROUP BY clause.</param>
        /// <param name="valueColumn">Column that is summed.</param>
        /// <param name="constraints">WHERE clauses appended to the query.</param>
        public SelectGroupedValue(
            string table,
            string groupColumn,
            string valueColumn,
            ImmutableList<Where> constraints)
        {
            Table = table;
            GroupColumn = groupColumn;
            ValueColumn = valueColumn;
            Constraints = constraints;
        }

        /// <summary>
        /// Gets the WHERE clauses applied to the query.
        /// </summary>
        public readonly ImmutableList<Where> Constraints;

        /// <summary>
        /// Gets the column used for grouping.
        /// </summary>
        public readonly string GroupColumn;

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public readonly string Table;

        /// <summary>
        /// Gets the column that is summed.
        /// </summary>
        public readonly string ValueColumn;

        /// <inheritdoc />
        public GroupedValue<TGroup, TValue> TranslateRow(ISqlFields fields)
        {
            return new GroupedValue<TGroup, TValue>(
                ConvertValue<TGroup>(fields.Get(0)),
                ConvertValue<TValue>(fields.Get(1)));
        }

        /// <inheritdoc />
        public SqlCommandText Format(SqlFormat format)
        {
            var builder = new StringBuilder();
            var parameters = ImmutableArray.CreateBuilder<SqlParameterValue>();
            string groupColumn = SqlIdentifier.Quote(GroupColumn, format);
            string valueColumn = SqlIdentifier.Quote(ValueColumn, format);
            string table = SqlIdentifier.Quote(Table, format);

            builder.AppendFormat("SELECT {0}, SUM({1}) ", groupColumn, valueColumn);
            builder.AppendFormat("FROM {0}", table);

            bool isFirstConstraint = true;

            foreach (var constraint in Constraints)
            {
                builder.AppendLine();
                builder.Append(constraint.Format(format, isFirstConstraint));
                parameters.AddRange(constraint.Parameters);
                isFirstConstraint = false;
            }

            builder.AppendLine();
            builder.AppendFormat("GROUP BY {0}", groupColumn);

            return new SqlCommandText(builder.ToString(), parameters.ToImmutable());
        }

        /// <summary>
        /// Converts database values to the requested CLR type when possible.
        /// </summary>
        /// <typeparam name="T">Requested result type.</typeparam>
        /// <param name="value">Database value.</param>
        /// <returns>Converted value or <see langword="default"/> if conversion fails.</returns>
        private static T? ConvertValue<T>(object? value)
        {
            if (value == null || value is DBNull)
            {
                return default;
            }

            if (value is T expectedValue)
            {
                return expectedValue;
            }

            Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            if (value is IConvertible && typeof(IConvertible).IsAssignableFrom(targetType))
            {
                object converted = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                return (T)converted;
            }

            return default;
        }
    }
}
