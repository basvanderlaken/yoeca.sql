using System;

namespace Yoeca.Sql.Tests.Integration
{
    [TableDefinition("IdentifiedBlobs")]
    public sealed class IdentifiedBlob
    {
        [SqlPrimaryKey]
        public Guid Identifier { get; set; }

        [SqlNotNull]
        public Payload Value { get; set; }
    }
}