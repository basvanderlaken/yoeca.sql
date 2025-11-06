namespace Yoeca.Sql.Tests.Integration
{
    internal static class MySqlTestDatabase
    {
        public static readonly string ConnectionString = Environment.GetEnvironmentVariable("YOECA_SQL_TESTDATABASE", EnvironmentVariableTarget.User) ?? string.Empty;
    }
}