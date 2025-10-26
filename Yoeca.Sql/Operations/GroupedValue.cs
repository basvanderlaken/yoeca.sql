namespace Yoeca.Sql
{
    /// <summary>
    /// Represents a grouped aggregate result where a group key is paired with an aggregated value.
    /// </summary>
    /// <typeparam name="TGroup">Type of the grouping key.</typeparam>
    /// <typeparam name="TValue">Type of the aggregated value.</typeparam>
    /// <param name="Group">Grouping key.</param>
    /// <param name="Value">Aggregated value.</param>
    public sealed record GroupedValue<TGroup, TValue>(TGroup? Group, TValue? Value);
}
