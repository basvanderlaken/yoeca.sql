namespace Yoeca.Sql
{
    /// <summary>
    /// Represents a named parameter bound to a SQL command.
    /// </summary>
    public sealed class SqlParameterValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlParameterValue"/> class.
        /// </summary>
        /// <param name="name">Name of the parameter, including the <c>@</c> prefix.</param>
        /// <param name="value">Raw value associated with the parameter.</param>
        public SqlParameterValue(string name, object? value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Gets the value assigned to the parameter.
        /// </summary>
        public object? Value
        {
            get;
        }
    }
}
