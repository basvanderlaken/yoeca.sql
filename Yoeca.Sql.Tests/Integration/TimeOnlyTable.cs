using System;

namespace Yoeca.Sql.Tests.Integration
{
    [SqlTableDefinition("timeonly_table")]
    internal sealed class TimeOnlyTable
    {
        [SqlPrimaryKey]
        public int Id
        {
            get;
            set;
        }

        [SqlTimeFraction(6)]
        public TimeOnly Value
        {
            get;
            set;
        }
    }
}
