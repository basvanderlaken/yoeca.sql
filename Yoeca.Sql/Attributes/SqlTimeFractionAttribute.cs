namespace Yoeca.Sql
{
    /// <summary>
    /// Attribute to specify the number of decimals to store. Must be a value between 0 and 6.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SqlTimeFractionAttribute (int numberOfDecimals): Attribute
    {
        /// <summary>
        /// Gets the number of decimals to use for time fractions.
        /// </summary>
        public int NumberOfDecimals { get; } = numberOfDecimals;
    }
}