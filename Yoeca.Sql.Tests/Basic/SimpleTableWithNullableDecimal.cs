using Yoeca.Sql;

namespace Yoeca.Sql.Tests.Basic
{
    [SqlTableDefinition("simple_nullable_decimal")]
    internal sealed class SimpleTableWithNullableDecimal
    {
        public decimal? Value
        {
            get; set;
        }
    }
}
