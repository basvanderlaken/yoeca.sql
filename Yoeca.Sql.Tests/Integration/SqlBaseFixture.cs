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
            Connection = ConnectionFactory.MySql(MySqlTestDatabase.ConnectionString);
        }
    }
}