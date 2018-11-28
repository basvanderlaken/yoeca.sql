using System;

namespace Yoeca.Sql.Tests.Integration
{
    [SqlTableDefinition("players")]
    public sealed class Player
    {
        [SqlPrimaryKey]
        public Guid Identifier
        {
            get;
            set;
        }

        [SqlNotNull]
        public string Name
        {
            get;
            set;
        }

        public int Age
        {
            get;
            set;
        }

        public DateTime Birthday
        {
            get;
            set;
        }
    }
}