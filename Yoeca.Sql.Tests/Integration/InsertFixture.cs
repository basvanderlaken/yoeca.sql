using System;
using System.Collections.Immutable;
using NUnit.Framework;

namespace Yoeca.Sql.Tests.Integration
{
    [TestFixture]
    internal sealed class InsertFixture : SqlBaseFixture
    {
        [Test]
        public void WhenRecordsInsertedTheyCanGetUpdated()
        {
            DropTable.For<Player>().TryExecute(Connection);
            CreateTable.For<Player>().Execute(Connection);

            var player = new Player
            {
                Age = 10,
                Birthday = DateTime.Now,
                Identifier = Guid.NewGuid(),
                Name = "Jim"
            };

            Assert.That(InsertInto.Row(player).TryExecute(Connection));
            var records = Select.From<Player>().ExecuteRead(Connection).ToImmutableList();
            Assert.That(records, Has.Count.EqualTo(1));
            Assert.That(records[0].Name, Is.EqualTo("Jim"));

            player.Name = "John";
            Assert.That(InsertInto.Row(player).TryExecute(Connection), Is.False);

            records = Select.From<Player>().ExecuteRead(Connection).ToImmutableList();
            Assert.That(records, Has.Count.EqualTo(1));
            Assert.That(records[0].Name, Is.EqualTo("Jim"));

            Assert.That(InsertInto.Row(player).UpdateOnDuplicateKey.TryExecute(Connection));

            records = Select.From<Player>().ExecuteRead(Connection).ToImmutableList();
            Assert.That(records, Has.Count.EqualTo(1));
            Assert.That(records[0].Name, Is.EqualTo("John"));
        }
    }
}