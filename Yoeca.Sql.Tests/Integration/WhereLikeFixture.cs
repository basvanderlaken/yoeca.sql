using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Yoeca.Sql.Tests.Integration
{
    [TestFixture]
    internal sealed class WhereLikeFixture : SqlBaseFixture
    {
        private Player Create(string name)
        {
            return new Player
            {
                Age = 10,
                Birthday = DateTime.UtcNow,
                Identifier = Guid.NewGuid(),
                Name = name
            };
        }

        [Test]
        public async Task VerifyHasTableWorks()
        {
            DropTable.For<Player>().TryExecute(Connection);

            await Table.Ensure<Player>(Connection);

            await InsertInto.Row(Create("Willem")).ExecuteAsync(Connection);
            await InsertInto.Row(Create("Willemse")).ExecuteAsync(Connection);
            await InsertInto.Row(Create("Bastiaan")).ExecuteAsync(Connection);
            await InsertInto.Row(Create("Harry")).ExecuteAsync(Connection);

            Console.WriteLine(Select<Player>.All().WhereContains(x => x.Name, "illem").Format(SqlFormat.MySql));
            var result = Select<Player>.All().WhereContains(x => x.Name, "illem").ExecuteRead(Connection).ToList();

            Assert.That(result, Has.Count.EqualTo(2));

            result = Select<Player>.All().WhereStartsWith(x => x.Name, "bas").ExecuteRead(Connection).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Bastiaan"));

            result = Select<Player>.All().WhereEndsWith(x => x.Name, "ry").ExecuteRead(Connection).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Harry"));
        }
    }
}