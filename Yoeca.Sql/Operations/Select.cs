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
        [ItemNotNull]
        [NotNull]
        public readonly ImmutableList<Where> Constraints;

        [CanBeNull]
        public readonly int? Limit;

        [NotNull]
        [ItemNotNull]
        public readonly ImmutableList<string> Parameters;

        [NotNull]
        public readonly string Table;

        public Select(
            [NotNull] string table,
            [NotNull, ItemNotNull] ImmutableList<string> values,
            [NotNull, ItemNotNull] ImmutableList<Where> constraints,
            [CanBeNull] int? limit)
        {
            Table = table;
            Parameters = values;
            Constraints = constraints;
            Limit = limit;
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

            if (Limit.HasValue)
            {
                builder.AppendLine();
                builder.AppendFormat("LIMIT {0}", Limit.Value);
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

            return new Select<T>(definition.Name, parameters.ToImmutableList(), ImmutableList<Where>.Empty, null);
        }

        [NotNull]
        public Select<T> WhereEqual<TResult>([NotNull] Expression<Func<T, TResult>> expression, [NotNull] TResult value)
        {
            var column = GetColumn(expression);
            string formattedValue = column.Convert.ConvertToString(value);

            if (formattedValue == null)
            {
                throw new ArgumentException("Specified value cannot be converted to column value: " + value);
            }

            if (column.RequiresEscaping)
            {
                formattedValue = "'" + formattedValue + "'";
            }

            return With(new WhereEqual(column.Name, formattedValue));
        }

        [NotNull]
        public Select<T> WhereContains([NotNull] Expression<Func<T, string>> expression, [NotNull] string value)
        {
            var column = GetColumn(expression);
            string formattedValue = "'%" + value + "%'";

            return With(new WhereLike(column.Name, formattedValue));
        }

        [NotNull]
        public Select<T> WhereStartsWith([NotNull] Expression<Func<T, string>> expression, [NotNull] string value)
        {
            var column = GetColumn(expression);
            string formattedValue = "'" + value + "%'";

            return With(new WhereLike(column.Name, formattedValue));
        }

        [NotNull]
        public Select<T> WhereEndsWith([NotNull] Expression<Func<T, string>> expression, [NotNull] string value)
        {
            var column = GetColumn(expression);
            string formattedValue = "'%" + value + "'";

            return With(new WhereLike(column.Name, formattedValue));
        }

        [NotNull]
        private Select<T> With([NotNull] Where constraint)
        {
            return new Select<T>(Table, Parameters, Constraints.Add(constraint), Limit);
        }

        [NotNull]
        public Select<T> WhereNotEqual<TResult>(
            [NotNull] Expression<Func<T, TResult>> expression,
            [NotNull] TResult value)
        {
            var column = GetColumn(expression);
            string formattedValue = column.Convert.ConvertToString(value);

            if (column.RequiresEscaping)
            {
                formattedValue = "'" + formattedValue + "'";
            }

            return With(new WhereNotEqual(column.Name, formattedValue));
        }

        [NotNull]
        public Select<T> Take(int maximumNumberOfRecords)
        {
            return new Select<T>(Table, Parameters, Constraints, maximumNumberOfRecords);
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
    }
}