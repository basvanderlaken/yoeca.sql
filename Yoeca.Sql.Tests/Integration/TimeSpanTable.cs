using System;

namespace Yoeca.Sql.Tests.Integration
{
    [SqlTableDefinition("timespan_table")]
    internal sealed class TimeSpanTable
    {
        [SqlPrimaryKey]
        public int Id
        {
            get;
            set;
        }

        public TimeSpan Value
        {
            get;
            set;
        }
    }
}
