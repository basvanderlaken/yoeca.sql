using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;

namespace Yoeca.Sql
{
    public sealed class Select
    {
        [NotNull]
        public static Select<T> From<T>() where T : new()
        {
            return Select<T>.All();
        }
    }

    public sealed class Select<T> : ISqlCommand<T>
        where T : new()
    {
        public readonly ImmutableList<Where> Constraints;
        public readonly ImmutableList<string> Parameters;
        public readonly string Table;

        public Select(
            [NotNull] string table,
            [NotNull] ImmutableList<string> values,
            [NotNull] ImmutableList<Where> constraints)
        {
            Table = table;
            Parameters = values;
            Constraints = constraints;
        }

        public T TranslateRow(ISqlFields fields)
        {
            var definition = new TableDefinition(typeof(T));

            var result = new T();

            int index = 0;
            foreach (var columnRetriever in definition.Columns)
            {
                columnRetriever.Set(fields.Get(index), result);
                index++;
            }

            return result;
        }

        public string Format(SqlFormat format)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("SELECT {0} ", string.Join(", ", Parameters));
            builder.AppendFormat("FROM {0}", Table);

            foreach (var constraint in Constraints)
            {
                builder.AppendLine();
                builder.Append(constraint.Format(format));
            }

            return builder.ToString();
        }

        [NotNull]
        public SelectValue<T, TValue> Maximum<TValue>([NotNull] Expression<Func<T, TValue>> expression)
        {
            var column = GetColumn(expression);

            return new SelectValue<T, TValue>(Table, column.Name, Constraints, ValueOperations.Maximum);
        }

        [NotNull]
        public SelectValue<T, TValue> Minimum<TValue>([NotNull] Expression<Func<T, TValue>> expression)
        {
            var column = GetColumn(expression);

            return new SelectValue<T, TValue>(Table, column.Name, Constraints, ValueOperations.Minimum);
        }

        [NotNull]
        public static Select<T> All()
        {
            var definition = new TableDefinition(typeof(T));

            var parameters = new List<string>();
            foreach (var columnRetriever in definition.Columns)
            {
                parameters.Add(columnRetriever.Name);
            }

            return new Select<T>(definition.Name, parameters.ToImmutableList(), ImmutableList<Where>.Empty);
        }

        public Select<T> WhereEqual<TResult>([NotNull] Expression<Func<T, TResult>> expression, [NotNull] TResult value)
        {
            var column = GetColumn(expression);
            string formattedValue = column.Convert.ConvertToString(value);

            if (column.RequiresEscaping)
            {
                formattedValue = "'" + formattedValue + "'";
            }

            return new Select<T>(Table, Parameters, Constraints.Add(new WhereEqual(column.Name, formattedValue)));
        }

        [NotNull]
        private static ColumnRetriever GetColumn<TResult>([NotNull] Expression<Func<T, TResult>> expression)
        {
            var member = expression.Body as MemberExpression;

            if (member == null)
            {
                throw new InvalidOperationException("Not a valid lambda.");
            }

            var definition = new TableDefinition(typeof(T));

            return definition.Columns.Single(x => x.Name == member.Member.Name);
        }

        [NotNull]
        public Select<T> WhereNotEqual<TResult>(
            [NotNull] Expression<Func<T, TResult>> expression,
            [NotNull] TResult value)
        {
            var member = expression.Body as MemberExpression;

            if (member == null)
            {
                throw new InvalidOperationException("Not a valid lambda.");
            }

            var definition = new TableDefinition(typeof(T));

            var column = definition.Columns.Single(x => x.Name == member.Member.Name);

            string formattedValue = column.Convert.ConvertToString(value);

            Type valueType = typeof(TResult);
            if (valueType == typeof(string) || valueType == typeof(Guid))
            {
                formattedValue = "'" + formattedValue + "'";
            }

            return new Select<T>(Table,
                Parameters,
                Constraints.Add(new WhereNotEqual(member.Member.Name, formattedValue)));
        }
    }
}