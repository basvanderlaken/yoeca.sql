using System;

namespace Yoeca.Sql.Tests.Integration
{
    [SqlTableDefinition("nullable_guid")]
    public sealed class NullableGuidTable
    {
        [SqlPrimaryKey]
        public int Id { get; set; }

        public Guid? OptionalIdentifier { get; set; }
    }
}
