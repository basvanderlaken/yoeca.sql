using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;


namespace Yoeca.Sql
{
    public sealed class SelectValue<TTable, TValue> : ISqlCommand<TValue>
    {
        
        
        public readonly ImmutableList<Where> Constraints;

        public readonly ValueOperations Operation;

        
        public readonly string Parameter;

        
        public readonly string Table;

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

        public TValue? TranslateRow(ISqlFields fields)
        {
            return ConvertValue(fields.Get(0));
        }

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
    }
}
