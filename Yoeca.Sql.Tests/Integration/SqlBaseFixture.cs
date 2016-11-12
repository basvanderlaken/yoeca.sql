using JetBrains.Annotations;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace Yoeca.Sql.Tests.Integration
{
    internal abstract class SqlBaseFixture
    {
        [NotNull]
        protected MySqlConnection Connection { get; private set; }

        [SetUp]
        public void Setup()
        {
            Connection = new MySqlConnection(MySqlTestDatabase.ConnectionString);

            Connection.Open();
        }

        [TearDown]
        public void Teardwon()
        {
            Connection.Dispose();
        }
    }
}