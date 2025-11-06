using NUnit.Framework;

namespace Yoeca.Sql.Tests.Integration
{
    internal abstract class SqlBaseFixture
    {
        protected ISqlConnection Connection
        {
            get;
            private set;
        }

        [SetUp]
        public void Setup()
        {
            Assert.That (MySqlTestDatabase.ConnectionString, Is.Not.Null.And.Not.Empty, "The MySQL connection string must be set for integration tests.");
            Connection = ConnectionFactory.MySql(MySqlTestDatabase.ConnectionString);
        }
    }
}