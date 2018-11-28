using JetBrains.Annotations;

namespace Yoeca.Sql.NUnit
{
    [SqlTableDefinition("with_autoincrement")]
    internal sealed class TableWithIncrement
    {
        [SqlPrimaryKey, AutoIncrement]
        public ulong Identifier
        {
            get;
            set;
        }

        [SqlNotNull, MaximumSize(32)]
        [NotNull]
        public string Value
        {
            get;
            set;
        } = string.Empty;
    }
}