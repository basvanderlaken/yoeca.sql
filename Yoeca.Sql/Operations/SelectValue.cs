using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Yoeca.Sql
{
    /// <summary>
    /// Represents a SELECT command that returns a single aggregated value from a table.
    /// </summary>
    /// <typeparam name="TTable">The entity describing the table schema.</typeparam>
    /// <typeparam name="TValue">Type that will be produced by the aggregate function.</typeparam>
    public sealed class SelectValue<TTable, TValue> : ISqlCommand<TValue>
    {
        /// <summary>
        /// Gets the WHERE clauses appended to the query.
        /// </summary>
        public readonly ImmutableList<Where> Constraints;

        /// <summary>
        /// Gets the aggregate operation that will be rendered.
        /// </summary>
        public readonly ValueOperations Operation;

        private readonly ColumnRetriever mColumn;

        /// <summary>
        /// Gets the table that is queried.
        /// </summary>
        public readonly string Table;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectValue{TTable, TValue}"/> class.
        /// </summary>
        /// <param name="table">Name of the table.</param>
        /// <param name="parameter">Column name that should be aggregated.</param>
        /// <param name="constraints">Where conditions applied to the query.</param>
        /// <param name="operation">Aggregate operation to execute.</param>
        public SelectValue(
            string table,
            string parameter,
            ImmutableList<Where> constraints,
            ValueOperations operation)
            : this(table, ResolveColumn(parameter), constraints, operation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectValue{TTable, TValue}"/> class.
        /// </summary>
        /// <param name="table">Name of the table.</param>
        /// <param name="column">Column metadata for the aggregation.</param>
        /// <param name="constraints">Where conditions applied to the query.</param>
        /// <param name="operation">Aggregate operation to execute.</param>
        internal SelectValue(
            string table,
            ColumnRetriever column,
            ImmutableList<Where> constraints,
            ValueOperations operation)
        {
            Table = table;
            mColumn = column;
            Constraints = constraints;
            Operation = operation;
        }

        /// <inheritdoc />
        public TValue? TranslateRow(ISqlFields fields)
        {
            return ConvertValue(fields.Get(0));
        }

        /// <inheritdoc />
        public string Format(SqlFormat format)
        {
            var builder = new StringBuilder();
            string parameter = SqlIdentifier.Quote(mColumn.Name, format);
            string table = SqlIdentifier.Quote(Table, format);

            switch (Operation)
            {
                case ValueOperations.Maximum:
                    builder.AppendFormat("SELECT MAX({0}) ", parameter);
                    break;
                case ValueOperations.Minimum:
                    builder.AppendFormat("SELECT MIN({0}) ", parameter);
                    break;
                case ValueOperations.Sum:
                    builder.AppendFormat("SELECT SUM({0}) ", parameter);
                    break;
                default:
                    throw new NotSupportedException("Unsupported seelect operation: " + Operation);
            }

            builder.AppendFormat("FROM {0}", table);

            bool isFirstConstraint = true;

            foreach (var constraint in Constraints)
            {
                builder.AppendLine();
                builder.Append(constraint.Format(format, isFirstConstraint));
                isFirstConstraint = false;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts the raw value coming from <see cref="ISqlFields"/> into the requested <typeparamref name="TValue"/>.
        /// </summary>
        /// <param name="value">Raw database value.</param>
        /// <returns>The converted value when possible; otherwise <see langword="default"/>.</returns>
        private TValue? ConvertValue(object? value)
        {
            if (value == null || value is DBNull)
            {
                return default;
            }

            if (value is TValue expectedValue)
            {
                return expectedValue;
            }

            Type targetType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

            TypeConverter columnConverter = mColumn.Convert;

            object? columnConverted = TryConvert(columnConverter, value);

            if (columnConverted is TValue typedFromColumn)
            {
                return typedFromColumn;
            }

            var typeConverter = TypeDescriptor.GetConverter(targetType);

            object? convertedByType = TryConvert(typeConverter, value);

            if (convertedByType is TValue typedFromTypeConverter)
            {
                return typedFromTypeConverter;
            }

            if (value is IConvertible && typeof(IConvertible).IsAssignableFrom(targetType))
            {
                object converted = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                return (TValue)converted;
            }

            return default;
        }

        private static object? TryConvert(TypeConverter converter, object value)
        {
            try
            {
                if (converter.CanConvertFrom(value.GetType()))
                {
                    return converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
                }

                if (converter.CanConvertFrom(typeof(string)))
                {
                    string? serialized = Convert.ToString(value, CultureInfo.InvariantCulture);

                    if (!string.IsNullOrEmpty(serialized))
                    {
                        return converter.ConvertFrom(null, CultureInfo.InvariantCulture, serialized);
                    }
                }
            }
            catch (Exception)
            {
                // Ignore conversion failures and fall back to alternative strategies.
            }

            return null;
        }

        private static ColumnRetriever ResolveColumn(string columnName)
        {
            var definition = new TableDefinition(typeof(TTable));
            return definition.Columns.Single(column => column.Name == columnName);
        }
    }
}


