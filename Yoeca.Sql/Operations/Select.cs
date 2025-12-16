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

        public SqlCommandText Format(SqlFormat format)
        {
            var builder = new StringBuilder();
            var parameters = ImmutableArray.CreateBuilder<SqlParameterValue>();

            builder.AppendFormat("SELECT {0} ",
                                 string.Join(", ", SqlIdentifier.Quote(Parameters, format)));
            builder.AppendFormat("FROM {0}", SqlIdentifier.Quote(Table, format));

            bool isFirstConstraint = true;

            foreach (var constraint in Constraints)
            {
                builder.AppendLine();
                builder.Append(constraint.Format(format, isFirstConstraint));
                parameters.AddRange(constraint.Parameters);
                isFirstConstraint = false;
            }

            if (Limit.HasValue)
            {
                builder.AppendLine();
                builder.AppendFormat("LIMIT {0}", Limit.Value);
            }

            return new SqlCommandText(builder.ToString(), parameters.ToImmutable());
        }

        public SelectValue<T, TValue> Maximum<TValue>( Expression<Func<T, TValue>> expression)
        {
            var column = GetColumn(expression);

            return new SelectValue<T, TValue>(Table, column, Constraints, ValueOperations.Maximum);
        }

        
        public SelectValue<T, TValue> Minimum<TValue>( Expression<Func<T, TValue>> expression)
        {
            var column = GetColumn(expression);

            return new SelectValue<T, TValue>(Table, column, Constraints, ValueOperations.Minimum);
        }

        /// <summary>
        /// Creates a command that calculates the sum of the specified column.
        /// </summary>
        /// <typeparam name="TValue">Type of the column result.</typeparam>
        /// <param name="expression">Lambda pointing to the column that needs to be aggregated.</param>
        /// <returns>A command that selects the sum value.</returns>
        public SelectValue<T, TValue> Sum<TValue>(Expression<Func<T, TValue>> expression)
        {
            var column = GetColumn(expression);

            return new SelectValue<T, TValue>(Table, column, Constraints, ValueOperations.Sum);
        }

        /// <summary>
        /// Creates a command that calculates the sum of a column grouped by another column.
        /// </summary>
        /// <typeparam name="TValue">Type of the column that is summed.</typeparam>
        /// <typeparam name="TGroup">Type of the column that is used for the grouping.</typeparam>
        /// <param name="valueExpression">Expression resolving the column to aggregate.</param>
        /// <param name="groupExpression">Expression resolving the column to group on.</param>
        /// <returns>A command that selects grouped sum values.</returns>
        public SelectGroupedValue<T, TGroup, TValue> SumBy<TValue, TGroup>(
            Expression<Func<T, TValue>> valueExpression,
            Expression<Func<T, TGroup>> groupExpression)
        {
            var valueColumn = GetColumn(valueExpression);
            var groupColumn = GetColumn(groupExpression);

            return new SelectGroupedValue<T, TGroup, TValue>(Table, groupColumn.Name, valueColumn.Name, Constraints);
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
            var parameterName = CreateParameterName();
            var parameterValue = CreateParameterValue(column, value);

            return With(new WhereEqual(column.Name, parameterName, parameterValue));
        }

        public Select<T> WhereGreaterOrEqual<TResult>(Expression<Func<T, TResult>> expression, TResult value)
        {
            var column = GetColumn(expression);
            var parameterName = CreateParameterName();
            var parameterValue = CreateParameterValue(column, value);

            return With(new WhereGreaterOrEqual(column.Name, parameterName, parameterValue));
        }

        public Select<T> WhereLess<TResult>(Expression<Func<T, TResult>> expression, TResult value)
        {
            var column = GetColumn(expression);
            var parameterName = CreateParameterName();
            var parameterValue = CreateParameterValue(column, value);

            return With(new WhereLess(column.Name, parameterName, parameterValue));
        }

        
        public Select<T> WhereContains(Expression<Func<T, string>> expression,  string value)
        {
            var column = GetColumn(expression);
            string parameterName = CreateParameterName();
            string formattedValue = CreateParameterValue(column, value, v => "%" + v + "%").ToString();

            return With(new WhereLike(column.Name, parameterName, formattedValue));
        }

        
        public Select<T> WhereStartsWith( Expression<Func<T, string>> expression,  string value)
        {
            var column = GetColumn(expression);
            string parameterName = CreateParameterName();
            string formattedValue = CreateParameterValue(column, value, v => v + "%").ToString();

            return With(new WhereLike(column.Name, parameterName, formattedValue));
        }

        
        public Select<T> WhereEndsWith( Expression<Func<T, string>> expression,  string value)
        {
            var column = GetColumn(expression);
            string parameterName = CreateParameterName();
            string formattedValue = CreateParameterValue(column, value, v => "%" + v).ToString();

            return With(new WhereLike(column.Name, parameterName, formattedValue));
        }

        
        public Select<T> WhereDayOfWeek(Expression<Func<T, DateOnly>> expression, DayOfWeek dayOfWeek)
        {
            var column = GetColumn(expression);

            if (column.Property.PropertyType != typeof(DateOnly))
            {
                throw new NotSupportedException("WhereDayOfWeek is only supported for DateOnly columns.");
            }

            int mysqlDayOfWeek = ((int)dayOfWeek) + 1;

            return With(new WhereDayOfWeek(column.Name, mysqlDayOfWeek));
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
            var parameterName = CreateParameterName();
            var parameterValue = CreateParameterValue(column, value);

            return With(new WhereNotEqual(column.Name, parameterName, parameterValue));
        }

        
        public Select<T> Take(int maximumNumberOfRecords)
        {
            return new Select<T>(Table, Parameters, Constraints, maximumNumberOfRecords);
        }

        public Select<T> WhereIn<TValue>(Expression<Func<T, TValue>> expression, IEnumerable<TValue> values)
        {
            ArgumentNullException.ThrowIfNull(values);

            var valueArray = values.ToImmutableArray();

            if (valueArray.IsEmpty)
            {
                throw new ArgumentException("At least one value must be provided.", nameof(values));
            }

            var column = GetColumn(expression);
            int startingIndex = CountParameters();
            var parameterNames = valueArray.Select((_, index) => CreateParameterName(startingIndex + index))
                .ToImmutableArray();
            var parameterValues = valueArray.Select(value => CreateParameterValue(column, value)).Cast<object?>().ToImmutableArray();

            return With(new WhereIn(column.Name, parameterNames, parameterValues));
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

        private static object CreateParameterValue<TValue>(ColumnRetriever column, TValue value, Func<string, string>? formatter = null)
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

        private static string CreateParameterName(int parameterIndex)
        {
            return $"@p{parameterIndex}";
        }

        private int CountParameters()
        {
            return Constraints.Sum(constraint => constraint.Parameters.Length);
        }
    }
}
