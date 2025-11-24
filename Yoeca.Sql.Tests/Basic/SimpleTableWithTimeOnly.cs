using System;

namespace Yoeca.Sql.Tests.Basic
{
    [SqlTableDefinition("simple_timeonly")]
    internal sealed class SimpleTableWithTimeOnly
    {
        public TimeOnly Value
        {
            get;
            set;
        }
    }
}
