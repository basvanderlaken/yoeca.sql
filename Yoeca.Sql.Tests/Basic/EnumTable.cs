namespace Yoeca.Sql.NUnit
{
    [SqlTableDefinition("enumtable")]
    internal sealed class EnumTable
    {
        [SqlNotNull, MaximumSize(128), SqlPrimaryKey]
        public string Name { get; set; }

        public Something Something { get; set; }
    }

    public enum Something
    {
        First,
        Second,
        Third
    }
}