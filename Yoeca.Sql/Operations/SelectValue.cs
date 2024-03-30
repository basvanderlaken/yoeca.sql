using System;
using System.Collections.Immutable;
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
            if (fields.Get(0) is TValue expectedValue)
            {
                return expectedValue;
            }
            return default(TValue);
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