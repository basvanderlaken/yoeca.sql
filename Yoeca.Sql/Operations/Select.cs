using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Yoeca.Sql
{
    public sealed class Select
    {
        public static Select<T> From<T>()
            where T : new()
        {
            return Select<T>.All();
        }
    }

    public sealed class Select<T> : ISqlCommand<T>
        where T : new()
    {
        public readonly ImmutableList<Where> Constraints;

        public readonly int? Limit;

        public readonly ImmutableList<string> Parameters;

        public readonly string Table;

        public Select(
            string table,
            ImmutableList<string> values,
            ImmutableList<Where> constraints,
            int? limit)
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

        public SelectValue<T, TValue> Maximum<TValue>( Expression<Func<T, TValue>> expression)
        {
            var column = GetColumn(expression);

            return new SelectValue<T, TValue>(Table, column.Name, Constraints, ValueOperations.Maximum);
        }

        
        public SelectValue<T, TValue> Minimum<TValue>( Expression<Func<T, TValue>> expression)
        {
            var column = GetColumn(expression);

            return new SelectValue<T, TValue>(Table, column.Name, Constraints, ValueOperations.Minimum);
        }

        
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

        
        public Select<T> WhereEqual<TResult>( Expression<Func<T, TResult>> expression,  TResult value)
        {
            var column = GetColumn(expression);
            string? formattedValue = column.Convert.ConvertToString(value);

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

        
        public Select<T> WhereContains( Expression<Func<T, string>> expression,  string value)
        {
            var column = GetColumn(expression);
            string formattedValue = "'%" + value + "%'";

            return With(new WhereLike(column.Name, formattedValue));
        }

        
        public Select<T> WhereStartsWith( Expression<Func<T, string>> expression,  string value)
        {
            var column = GetColumn(expression);
            string formattedValue = "'" + value + "%'";

            return With(new WhereLike(column.Name, formattedValue));
        }

        
        public Select<T> WhereEndsWith( Expression<Func<T, string>> expression,  string value)
        {
            var column = GetColumn(expression);
            string formattedValue = "'%" + value + "'";

            return With(new WhereLike(column.Name, formattedValue));
        }

        
        private Select<T> With( Where constraint)
        {
            return new Select<T>(Table, Parameters, Constraints.Add(constraint), Limit);
        }

        
        public Select<T> WhereNotEqual<TResult>(
             Expression<Func<T, TResult>> expression,
             TResult value)
        {
            var column = GetColumn(expression);
            string? formattedValue = column.Convert.ConvertToString(value);

            if (formattedValue == null)
            {
                throw new ArgumentException("Specified value cannot be converted to column value: " + value);
            }

            if (column.RequiresEscaping)
            {
                formattedValue = "'" + formattedValue + "'";
            }

            return With(new WhereNotEqual(column.Name, formattedValue));
        }

        
        public Select<T> Take(int maximumNumberOfRecords)
        {
            return new Select<T>(Table, Parameters, Constraints, maximumNumberOfRecords);
        }

        
        private static ColumnRetriever GetColumn<TResult>( Expression<Func<T, TResult>> expression)
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