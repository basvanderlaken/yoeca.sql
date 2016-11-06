using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace Yoeca.Sql.Tests.Integration
{
    [TestFixture]
    internal sealed class HasTableFixture
    {
        [Test]
        public void VerifyHasTableWorks()
        {
            using (var connection = new MySqlConnection(MySqlTestDatabase.ConnectionString))
            {
                connection.Open();

                DropTable.For<Player>().TryExecute(connection);
                Assert.That(HasTable.For<Player>().ExecuteCheck(connection), Is.False);
                CreateTable.For<Player>().Execute(connection);
                Assert.That(HasTable.For<Player>().ExecuteCheck(connection), Is.True);
            }
        }
    }
}