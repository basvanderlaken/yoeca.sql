using Yoeca.Sql;

namespace Yoeca.Sql.Tests.Integration
{
    [SqlTableDefinition("nullable_decimal")]
    public sealed class NullableDecimalTable
    {
        [SqlPrimaryKey]
        public int Id
        {
            get; set;
        }

        public decimal? Value
        {
            get; set;
        }
    }
}
