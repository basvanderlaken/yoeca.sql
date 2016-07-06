using System;

namespace Yoeca.Sql.Tests.Integration
{
    [SqlTableDefinition("IdentifiedBlobs")]
    public sealed class IdentifiedBlob
    {
        [SqlPrimaryKey]
        public Guid Identifier { get; set; }

        [SqlNotNull]
        public Payload Value { get; set; }
    }
}