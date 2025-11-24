using System;

namespace Yoeca.Sql.Tests.Basic
{
    [SqlTableDefinition("simple_dateonly")]
    internal sealed class SimpleTableWithDateOnly
    {
        public DateOnly Value
        {
            get;
            set;
        }
    }
}
