using System;

namespace Yoeca.Sql.Tests.Basic
{
    [SqlTableDefinition("simple_timespan")]
    internal sealed class SimpleTableWithTimeSpan
    {
        public TimeSpan Value
        {
            get;
            set;
        }
    }
}
