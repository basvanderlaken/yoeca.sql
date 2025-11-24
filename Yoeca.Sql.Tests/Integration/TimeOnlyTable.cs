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

        public TimeOnly Value
        {
            get;
            set;
        }
    }
}
