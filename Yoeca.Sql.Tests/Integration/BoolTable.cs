using Yoeca.Sql;

namespace Yoeca.Sql.Tests.Integration
{
    [SqlTableDefinition("bool_table")]
    public sealed class BoolTable
    {
        [SqlPrimaryKey]
        public int Id
        {
            get; set;
        }

        [SqlNotNull]
        public bool Value
        {
            get; set;
        }
    }
}
