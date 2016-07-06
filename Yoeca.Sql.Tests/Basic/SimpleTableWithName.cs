namespace Yoeca.Sql.Tests.Basic
{
    [SqlTableDefinition("Simple")]
    internal sealed class SimpleTableWithName
    {
        public string Name { get; set; }
    }
}