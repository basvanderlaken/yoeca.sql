using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Yoeca.Sql
{
    /// <summary>
    /// Provides helpers for creating update commands.
    /// </summary>
    public static class Update
    {
        /// <summary>
        /// Creates an update command for the specified table definition.
        /// </summary>
        /// <typeparam name="T">Type representing the table definition.</typeparam>
        /// <returns>A new command targeting the requested table.</returns>
        public static Update<T> Table<T>()
            where T : new()
        {
            return Update<T>.Create();
        }
    }

    /// <summary>
    /// Builds an SQL update statement for the provided table definition.
    /// </summary>
    /// <typeparam name="T">Type describing the table columns.</typeparam>
    public sealed class Update<T> : ISqlCommand
        where T : new()
    {
        private Update(
            string table,
            ImmutableList<KeyValuePair<string, string>> assignments,
            ImmutableList<Where> constraints)
        {
            Table = table;
            Assignments = assignments;
            Constraints = constraints;
        }

        /// <summary>
        /// Gets the table associated with the update statement.
        /// </summary>
        public string Table
        {
            get;
        }

        /// <summary>
        /// Gets the assignment statements that will be executed.
        /// </summary>
        public ImmutableList<KeyValuePair<string, string>> Assignments
        {
            get;
        }

        /// <summary>
        /// Gets the where clauses configured for the update.
        /// </summary>
        public ImmutableList<Where> Constraints
        {
            get;
        }

        /// <summary>
        /// Creates a new command using the table metadata inferred from <typeparamref name="T" />.
        /// </summary>
        /// <returns>Empty update command.</returns>
        public static Update<T> Create()
        {
            var definition = new TableDefinition(typeof(T));

            return new Update<T>(definition.Name, ImmutableList<KeyValuePair<string, string>>.Empty, ImmutableList<Where>.Empty);
        }

        /// <summary>
        /// Formats the update command using the requested SQL dialect.
        /// </summary>
        /// <param name="format">Target SQL dialect.</param>
        /// <returns>Formatted SQL statement.</returns>
        public string Format(SqlFormat format)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("UPDATE {0} SET ", SqlIdentifier.Quote(Table, format));
            builder.Append(string.Join(", ", Assignments.Select(x => $"{SqlIdentifier.Quote(x.Key, format)} = {x.Value}")));

            bool isFirstConstraint = true;

            foreach (var constraint in Constraints)
            {
                builder.AppendLine();
                builder.Append(constraint.Format(format, isFirstConstraint));
                isFirstConstraint = false;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Adds or replaces an assignment for the provided column.
        /// </summary>
        /// <typeparam name="TValue">Type of the column value.</typeparam>
        /// <param name="expression">Expression selecting the column to update.</param>
        /// <param name="value">Value that will be stored.</param>
        /// <returns>Updated command builder.</returns>
        public Update<T> Set<TValue>(Expression<Func<T, TValue>> expression, TValue value)
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

            KeyValuePair<string, string> assignment = new(column.Name, formattedValue);

            int existingIndex = Assignments.FindIndex(x => x.Key == assignment.Key);
            var assignments = Assignments;

            if (existingIndex >= 0)
            {
                assignments = assignments.RemoveAt(existingIndex);
            }

            assignments = assignments.Add(assignment);
            return new Update<T>(Table, assignments, Constraints);
        }

        /// <summary>
        /// Adds an equality constraint to the update command.
        /// </summary>
        /// <typeparam name="TValue">Type of the column value.</typeparam>
        /// <param name="expression">Expression selecting the column for the constraint.</param>
        /// <param name="value">The value the column must match.</param>
        /// <returns>Updated command builder.</returns>
        public Update<T> WhereEqual<TValue>(Expression<Func<T, TValue>> expression, TValue value)
        {
            return With(CreateWhereEqual(expression, value));
        }

        /// <summary>
        /// Adds an inequality constraint to the update command.
        /// </summary>
        /// <typeparam name="TValue">Type of the column.</typeparam>
        /// <param name="expression">Expression selecting the column for the constraint.</param>
        /// <param name="value">Value the column must not match.</param>
        /// <returns>Updated command builder.</returns>
        public Update<T> WhereNotEqual<TValue>(Expression<Func<T, TValue>> expression, TValue value)
        {
            return With(CreateWhereNotEqual(expression, value));
        }

        /// <summary>
        /// Adds a greater-than-or-equal constraint to the update command.
        /// </summary>
        /// <typeparam name="TValue">Type of the column.</typeparam>
        /// <param name="expression">Expression selecting the column for the constraint.</param>
        /// <param name="value">Lower bound value to compare against.</param>
        /// <returns>Updated command builder.</returns>
        public Update<T> WhereGreaterOrEqual<TValue>(Expression<Func<T, TValue>> expression, TValue value)
        {
            return With(CreateWhereGreaterOrEqual(expression, value));
        }

        /// <summary>
        /// Adds a less-than constraint to the update command.
        /// </summary>
        /// <typeparam name="TValue">Type of the column.</typeparam>
        /// <param name="expression">Expression selecting the column for the constraint.</param>
        /// <param name="value">Upper bound value to compare against.</param>
        /// <returns>Updated command builder.</returns>
        public Update<T> WhereLess<TValue>(Expression<Func<T, TValue>> expression, TValue value)
        {
            return With(CreateWhereLess(expression, value));
        }

        /// <summary>
        /// Adds a contains constraint for text columns.
        /// </summary>
        /// <param name="expression">Expression selecting the column for the constraint.</param>
        /// <param name="value">Value to search for.</param>
        /// <returns>Updated command builder.</returns>
        public Update<T> WhereContains(Expression<Func<T, string>> expression, string value)
        {
            var column = GetColumn(expression);
            string formattedValue = "'%" + value + "%'";

            return With(new WhereLike(column.Name, formattedValue));
        }

        /// <summary>
        /// Adds a starts-with constraint for text columns.
        /// </summary>
        /// <param name="expression">Expression selecting the column for the constraint.</param>
        /// <param name="value">Value the column must start with.</param>
        /// <returns>Updated command builder.</returns>
        public Update<T> WhereStartsWith(Expression<Func<T, string>> expression, string value)
        {
            var column = GetColumn(expression);
            string formattedValue = "'" + value + "%'";

            return With(new WhereLike(column.Name, formattedValue));
        }

        /// <summary>
        /// Adds an ends-with constraint for text columns.
        /// </summary>
        /// <param name="expression">Expression selecting the column for the constraint.</param>
        /// <param name="value">Value the column must end with.</param>
        /// <returns>Updated command builder.</returns>
        public Update<T> WhereEndsWith(Expression<Func<T, string>> expression, string value)
        {
            var column = GetColumn(expression);
            string formattedValue = "'%" + value + "'";

            return With(new WhereLike(column.Name, formattedValue));
        }

        private Update<T> With(Where constraint)
        {
            return new Update<T>(Table, Assignments, Constraints.Add(constraint));
        }

        private static Where CreateWhereEqual<TValue>(Expression<Func<T, TValue>> expression, TValue value)
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

            return new WhereEqual(column.Name, formattedValue);
        }

        private static Where CreateWhereNotEqual<TValue>(Expression<Func<T, TValue>> expression, TValue value)
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

            return new WhereNotEqual(column.Name, formattedValue);
        }

        private static Where CreateWhereGreaterOrEqual<TValue>(Expression<Func<T, TValue>> expression, TValue value)
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

            return new WhereGreaterOrEqual(column.Name, formattedValue);
        }

        private static Where CreateWhereLess<TValue>(Expression<Func<T, TValue>> expression, TValue value)
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

            return new WhereLess(column.Name, formattedValue);
        }

        private static ColumnRetriever GetColumn<TValue>(Expression<Func<T, TValue>> expression)
        {
            if (expression.Body is not MemberExpression member)
            {
                throw new InvalidOperationException("Not a valid lambda.");
            }

            var definition = new TableDefinition(typeof(T));

            return definition.Columns.Single(x => x.Name == member.Member.Name);
        }
    }
}
