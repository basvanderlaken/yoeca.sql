
namespace Yoeca.Sql
{
    /// <summary>
    /// Represents an executable SQL command that can translate result rows.
    /// </summary>
    public interface ISqlCommand<T> : ISqlCommand
    {
        /// <summary>
        /// Projects a result row into a strongly typed instance.
        /// </summary>
        /// <param name="fields">Row data returned from the database.</param>
        /// <returns>Materialized instance or <c>null</c> if the row should be skipped.</returns>
        T? TranslateRow(ISqlFields fields);
    }

    /// <summary>
    /// Represents a formatted SQL command.
    /// </summary>
    public interface ISqlCommand
    {
        /// <summary>
        /// Formats the command for the provided SQL dialect and collects parameters.
        /// </summary>
        /// <param name="format">Target SQL dialect.</param>
        /// <returns>Formatted SQL text and parameter values.</returns>
        SqlCommandText Format(SqlFormat format);
    }
}
