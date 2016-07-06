namespace Yoeca.Sql.Tests.Basic
{
    [SqlTableDefinition("simple_double")]
    internal sealed class SimpleTableWithDouble
    {
        public double Value { get; set; }
    }
}