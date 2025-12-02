using Yoeca.Sql;

namespace Yoeca.Sql.Tests.Integration
{
    [SqlTableDefinition("nullable_bool")]
    public sealed class NullableBoolTable
    {
        [SqlPrimaryKey]
        public int Id
        {
            get; set;
        }

        public bool? Value
        {
            get; set;
        }
    }
}
