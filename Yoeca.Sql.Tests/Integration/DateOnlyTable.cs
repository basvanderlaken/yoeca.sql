using System;

namespace Yoeca.Sql.Tests.Integration
{
    [SqlTableDefinition("dateonly_table")]
    internal sealed class DateOnlyTable
    {
        [SqlPrimaryKey]
        public int Id
        {
            get;
            set;
        }

        public DateOnly Value
        {
            get;
            set;
        }
    }
}
