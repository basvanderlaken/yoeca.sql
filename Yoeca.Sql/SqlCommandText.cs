using System.Collections.Immutable;

namespace Yoeca.Sql
{
    /// <summary>
    /// Represents a fully formatted SQL command alongside its bound parameters.
    /// </summary>
    public sealed class SqlCommandText
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCommandText"/> class.
        /// </summary>
        /// <param name="command">Formatted SQL command text.</param>
        /// <param name="parameters">Parameters that accompany the SQL command.</param>
        public SqlCommandText(string command, ImmutableArray<SqlParameterValue> parameters)
        {
            Command = command;
            Parameters = parameters;
        }

        /// <summary>
        /// Gets the formatted SQL command text.
        /// </summary>
        public string Command
        {
            get;
        }

        /// <summary>
        /// Gets the collection of bound parameters.
        /// </summary>
        public ImmutableArray<SqlParameterValue> Parameters
        {
            get;
        }

        /// <summary>
        /// Creates an instance with the specified command text and no parameters.
        /// </summary>
        /// <param name="command">SQL command text.</param>
        /// <returns>Formatted SQL command without parameters.</returns>
        public static SqlCommandText WithoutParameters(string command)
        {
            return new SqlCommandText(command, ImmutableArray<SqlParameterValue>.Empty);
        }
    }
}
