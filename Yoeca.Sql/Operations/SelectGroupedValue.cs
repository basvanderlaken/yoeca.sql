using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;

namespace Yoeca.Sql
{
    public sealed class SelectGroupedValue<TTable, TGroup, TValue> : ISqlCommand<GroupedValue<TGroup, TValue>>
    {
        public readonly ImmutableList<Where> Constraints;

        public readonly string GroupColumn;

        public readonly string Table;

        public readonly string ValueColumn;

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

        public GroupedValue<TGroup, TValue> TranslateRow(ISqlFields fields)
        {
            return new GroupedValue<TGroup, TValue>(
                ConvertValue<TGroup>(fields.Get(0)),
                ConvertValue<TValue>(fields.Get(1)));
        }

        public string Format(SqlFormat format)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("SELECT {0}, SUM({1}) ", GroupColumn, ValueColumn);
            builder.AppendFormat("FROM {0}", Table);

            foreach (var constraint in Constraints)
            {
                builder.AppendLine();
                builder.Append(constraint.Format(format));
            }

            builder.AppendLine();
            builder.AppendFormat("GROUP BY {0}", GroupColumn);

            return builder.ToString();
        }

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
