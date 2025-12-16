using System;
using System.Linq;
using NUnit.Framework;

namespace Yoeca.Sql.Tests.Integration
{
    [TestFixture]
    internal sealed class StringEscapingFixture : SqlBaseFixture
    {
        [Test]
        public void WhenInsertingTextWithQuotesItRoundTrips()
        {
            DropTable.For<Player>().TryExecute(Connection);
            CreateTable.For<Player>().Execute(Connection);

            var trickyName = "Quote 'me' and backslash \\\\ plus percent %";

            var player = new Player
            {
                Identifier = Guid.NewGuid(),
                Name = trickyName,
                Age = 10,
                Birthday = DateTime.UtcNow
            };

            Assert.That(InsertInto.Row(player).TryExecute(Connection));

            var stored = Select.From<Player>()
                .WhereEqual(x => x.Identifier, player.Identifier)
                .ExecuteRead(Connection)
                .Single();

            Assert.That(stored.Name, Is.EqualTo(trickyName));
        }

        [Test]
        public void WhenUpdatingTextWithQuotesItRoundTrips()
        {
            DropTable.For<Player>().TryExecute(Connection);
            CreateTable.For<Player>().Execute(Connection);

            var player = new Player
            {
                Identifier = Guid.NewGuid(),
                Name = "Initial",
                Age = 20,
                Birthday = DateTime.UtcNow
            };

            Assert.That(InsertInto.Row(player).TryExecute(Connection));

            var updatedName = "New \\ name with 'single' quotes and % signs";

            Assert.That(
                Update.Table<Player>()
                    .Set(x => x.Name, updatedName)
                    .WhereEqual(x => x.Identifier, player.Identifier)
                    .TryExecute(Connection));

            var stored = Select.From<Player>()
                .WhereEqual(x => x.Identifier, player.Identifier)
                .ExecuteRead(Connection)
                .Single();

            Assert.That(stored.Name, Is.EqualTo(updatedName));
        }
    }
}
