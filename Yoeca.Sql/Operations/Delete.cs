using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Yoeca.Sql
{
    public static class Delete
    {
        public static Delete<T> From<T>()
            where T : new()
        {
            return Delete<T>.All();
        }
    }

    public sealed class Delete<T> : ISqlCommand<T>
        where T : new()
    {
        public readonly ImmutableList<Where> Constraints;

        public readonly string Table;

        public Delete(
            string table,
            ImmutableList<Where> constraints)
        {
            Table = table;
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

        public SqlCommandText Format(SqlFormat format)
        {
            var builder = new StringBuilder();
            var parameters = ImmutableArray.CreateBuilder<SqlParameterValue>();

            builder.AppendFormat("DELETE FROM {0} ", SqlIdentifier.Quote(Table, format));

            bool isFirstConstraint = true;

            foreach (var constraint in Constraints)
            {
                builder.AppendLine();
                builder.Append(constraint.Format(format, isFirstConstraint));
                parameters.AddRange(constraint.Parameters);
                isFirstConstraint = false;
            }

            return new SqlCommandText(builder.ToString(), parameters.ToImmutable());
        }

        
        public static Delete<T> All()
        {
            var definition = new TableDefinition(typeof(T));

            return new Delete<T>(definition.Name, ImmutableList<Where>.Empty);
        }

        
        public Delete<T> WhereEqual<TResult>( Expression<Func<T, TResult>> expression,  TResult value)
        {
            var column = GetColumn(expression);
            var parameterName = CreateParameterName();
            var formattedValue = CreateParameterValue(column, value);

            return With(new WhereEqual(column.Name, parameterName, formattedValue));
        }

        public Delete<T> WhereGreaterOrEqual<TResult>(Expression<Func<T, TResult>> expression, TResult value)
        {
            var column = GetColumn(expression);
            var formattedValue = CreateParameterValue(column, value);
            var parameterName = CreateParameterName();

            return With(new WhereGreaterOrEqual(column.Name, parameterName, formattedValue));
        }

        public Delete<T> WhereLess<TResult>(Expression<Func<T, TResult>> expression, TResult value)
        {
            var column = GetColumn(expression);
            var formattedValue = CreateParameterValue(column, value);
            var parameterName = CreateParameterName();

            return With(new WhereLess(column.Name, parameterName, formattedValue));
        }

        
        public Delete<T> WhereContains( Expression<Func<T, string>> expression,  string value)
        {
            var column = GetColumn(expression);
            string formattedValue = CreateParameterValue(column, value, v => "%" + v + "%");
            string parameterName = CreateParameterName();

            return With(new WhereLike(column.Name, parameterName, formattedValue));
        }

        
        public Delete<T> WhereStartsWith( Expression<Func<T, string>> expression,  string value)
        {
            var column = GetColumn(expression);
            string formattedValue = CreateParameterValue(column, value, v => v + "%");
            string parameterName = CreateParameterName();

            return With(new WhereLike(column.Name, parameterName, formattedValue));
        }

        
        public Delete<T> WhereEndsWith( Expression<Func<T, string>> expression,  string value)
        {
            var column = GetColumn(expression);
            string formattedValue = CreateParameterValue(column, value, v => "%" + v);
            string parameterName = CreateParameterName();

            return With(new WhereLike(column.Name, parameterName, formattedValue));
        }

        public Delete<T> WhereIn<TValue>(Expression<Func<T, TValue>> expression, IEnumerable<TValue> values)
        {
            ArgumentNullException.ThrowIfNull(values);

            var valueArray = values.ToImmutableArray();

            if (valueArray.IsEmpty)
            {
                throw new ArgumentException("At least one value must be provided.", nameof(values));
            }

            var column = GetColumn(expression);
            int startingIndex = CountParameters();
            var parameterNames = valueArray.Select((_, index) => CreateParameterName(startingIndex + index)).ToImmutableArray();
            var formattedValues = valueArray.Select(value => CreateParameterValue(column, value)).Cast<object?>().ToImmutableArray();

            return With(new WhereIn(column.Name, parameterNames, formattedValues));
        }

        
        private Delete<T> With( Where constraint)
        {
            return new Delete<T>(Table, Constraints.Add(constraint));
        }

        
        public Delete<T> WhereNotEqual<TResult>(
             Expression<Func<T, TResult>> expression,
             TResult value)
        {
            var column = GetColumn(expression);
            var parameterName = CreateParameterName();
            var formattedValue = CreateParameterValue(column, value);

            return With(new WhereNotEqual(column.Name, parameterName, formattedValue));
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

        private static string CreateParameterValue<TValue>(ColumnRetriever column, TValue value, Func<string, string>? formatter = null)
        {
            string? formattedValue = column.Convert.ConvertToString(value);

            if (formattedValue == null)
            {
                throw new ArgumentException("Specified value cannot be converted to column value: " + value);
            }

            if (formatter != null)
            {
                formattedValue = formatter(formattedValue);
            }

            return formattedValue;
        }

        private string CreateParameterName()
        {
            return CreateParameterName(CountParameters());
        }

        private static string CreateParameterName(int index)
        {
            return $"@p{index}";
        }

        private int CountParameters()
        {
            return Constraints.Sum(constraint => constraint.Parameters.Length);
        }
    }
}
