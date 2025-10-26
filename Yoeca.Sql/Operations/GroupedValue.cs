namespace Yoeca.Sql
{
    public sealed record GroupedValue<TGroup, TValue>(TGroup? Group, TValue? Value);
}
