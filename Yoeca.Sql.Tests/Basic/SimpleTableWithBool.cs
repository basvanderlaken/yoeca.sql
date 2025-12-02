using Yoeca.Sql;

namespace Yoeca.Sql.Tests.Basic
{
    [SqlTableDefinition("simple_bool")]
    internal sealed class SimpleTableWithBool
    {
        public bool Value
        {
            get; set;
        }
    }
}
