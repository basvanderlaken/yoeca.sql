namespace Yoeca.Sql.Tests.Integration
{
    [SqlTableDefinition("roles")]
    public class RolesTable
    {
        [SqlPrimaryKey]
        public int RoleIndex { get; set; }

        [SqlNotNull]
        [MaximumSize(100)]
        public string Name { get; set; } = string.Empty;
    }
}