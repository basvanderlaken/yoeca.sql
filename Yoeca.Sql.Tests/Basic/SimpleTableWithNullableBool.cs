using Yoeca.Sql;

namespace Yoeca.Sql.Tests.Basic
{
    [SqlTableDefinition("simple_nullable_bool")]
    internal sealed class SimpleTableWithNullableBool
    {
        public bool? Value
        {
            get; set;
        }
    }
}
