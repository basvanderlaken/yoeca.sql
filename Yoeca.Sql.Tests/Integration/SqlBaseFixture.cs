using JetBrains.Annotations;
using NUnit.Framework;

namespace Yoeca.Sql.Tests.Integration
{
    internal abstract class SqlBaseFixture
    {
        [NotNull]
        protected ISqlConnection Connection { get; private set; }

        [SetUp]
        public void Setup()
        {
            Connection = ConnectionFactory.MySql(MySqlTestDatabase.ConnectionString);
        }
    }
}