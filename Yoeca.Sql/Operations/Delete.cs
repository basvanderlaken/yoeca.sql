using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Text;

namespace Yoeca.Sql
{
    public sealed class Delete
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

        public string Format(SqlFormat format)
        {
            var builder = new StringBuilder();

            builder.AppendFormat($"DELETE FROM {Table} ");

            foreach (var constraint in Constraints)
            {
                builder.AppendLine();
                builder.Append(constraint.Format(format));
            }

            return builder.ToString();
        }

        
        public static Delete<T> All()
        {
            var definition = new TableDefinition(typeof(T));

            return new Delete<T>(definition.Name, ImmutableList<Where>.Empty);
        }

        
        public Delete<T> WhereEqual<TResult>( Expression<Func<T, TResult>> expression,  TResult value)
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

        
        public Delete<T> WhereContains( Expression<Func<T, string>> expression,  string value)
        {
            var column = GetColumn(expression);
            string formattedValue = "'%" + value + "%'";

            return With(new WhereLike(column.Name, formattedValue));
        }

        
        public Delete<T> WhereStartsWith( Expression<Func<T, string>> expression,  string value)
        {
            var column = GetColumn(expression);
            string formattedValue = "'" + value + "%'";

            return With(new WhereLike(column.Name, formattedValue));
        }

        
        public Delete<T> WhereEndsWith( Expression<Func<T, string>> expression,  string value)
        {
            var column = GetColumn(expression);
            string formattedValue = "'%" + value + "'";

            return With(new WhereLike(column.Name, formattedValue));
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