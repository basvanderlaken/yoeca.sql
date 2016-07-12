using System;
using System.Collections.Immutable;
using System.Text;
using JetBrains.Annotations;

namespace Yoeca.Sql
{
    public sealed class SelectValue<TTable, TValue> : ISqlCommand<TValue>
    {
        [ItemNotNull]
        [NotNull]
        public readonly ImmutableList<Where> Constraints;

        public readonly ValueOperations Operation;

        [NotNull]
        public readonly string Parameter;

        [NotNull]
        public readonly string Table;

        public SelectValue(
            [NotNull] string table,
            [NotNull] string parameter,
            [NotNull] [ItemNotNull] ImmutableList<Where> constraints,
            ValueOperations operation)
        {
            Table = table;
            Parameter = parameter;
            Constraints = constraints;
            Operation = operation;
        }

        public TValue TranslateRow(ISqlFields fields)
        {
            return (TValue) fields.Get(0);
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