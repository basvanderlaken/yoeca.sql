using System;

namespace Yoeca.Sql.Tests.Basic
{
    [SqlTableDefinition("simple_datetime")]
    internal sealed class SimpleTableWithDateTime
    {
        public DateTime Value { get; set; }
    }
}