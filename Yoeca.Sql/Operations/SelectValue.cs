using System;
using System.Collections.Immutable;
using System.Globalization;
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

        /// <summary>
        /// Gets the column that is part of the select.
        /// </summary>
        public readonly string Parameter;

        /// <summary>
        /// Gets the table that is queried.
        /// </summary>
        public readonly string Table;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectValue{TTable, TValue}"/> class.
        /// </summary>
        /// <param name="table">Name of the table.</param>
        /// <param name="parameter">Column that should be aggregated.</param>
        /// <param name="constraints">Where conditions applied to the query.</param>
        /// <param name="operation">Aggregate operation to execute.</param>
        public SelectValue(
             string table,
             string parameter,
              ImmutableList<Where> constraints,
            ValueOperations operation)
        {
            Table = table;
            Parameter = parameter;
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

            switch (Operation)
            {
                case ValueOperations.Maximum:
                    builder.AppendFormat("SELECT MAX({0}) ", Parameter);
                    break;
                case ValueOperations.Minimum:
                    builder.AppendFormat("SELECT MIN({0}) ", Parameter);
                    break;
                case ValueOperations.Sum:
                    builder.AppendFormat("SELECT SUM({0}) ", Parameter);
                    break;
                default:
                    throw new NotSupportedException("Unsupported seelect operation: " + Operation);
            }

            builder.AppendFormat("FROM {0}", Table);

            foreach (var constraint in Constraints)
            {
                builder.AppendLine();
                builder.Append(constraint.Format(format));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts the raw value coming from <see cref="ISqlFields"/> into the requested <typeparamref name="TValue"/>.
        /// </summary>
        /// <param name="value">Raw database value.</param>
        /// <returns>The converted value when possible; otherwise <see langword="default"/>.</returns>
        private static TValue? ConvertValue(object? value)
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

            if (value is IConvertible && typeof(IConvertible).IsAssignableFrom(targetType))
            {
                object converted = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                return (TValue)converted;
            }

            return default;
        }
    }
}
