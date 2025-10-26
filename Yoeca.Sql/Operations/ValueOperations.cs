namespace Yoeca.Sql
{
    /// <summary>
    /// Represents the supported aggregate functions that can be emitted by the <see cref="Select"/> helpers.
    /// </summary>
    public enum ValueOperations
    {
        /// <summary>
        /// Calculates the maximum value for the selected column.
        /// </summary>
        Maximum,

        /// <summary>
        /// Calculates the minimum value for the selected column.
        /// </summary>
        Minimum,

        /// <summary>
        /// Calculates the sum for the selected column.
        /// </summary>
        Sum
    }
}
