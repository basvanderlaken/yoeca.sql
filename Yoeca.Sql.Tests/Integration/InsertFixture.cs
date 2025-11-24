using System;
using System.Collections.Immutable;
using System.Linq;
using NUnit.Framework;
using Yoeca.Sql.NUnit;

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

        [Test]
        public void WhenRecordInsertedWithAutoIncrementIdentityGetsUpdated()
        {
            DropTable.For<TableWithIncrement>().TryExecute(Connection);
            CreateTable.For<TableWithIncrement>().Execute(Connection);

            var first = new TableWithIncrement
            {
                Value = "Foo"
            };

            var second = new TableWithIncrement
            {
                Value = "Bar"
            };

            var numberFirst = InsertInto.Row(first).GetLastInsertIdentity<ulong>().ExecuteRead(Connection).Single();
            var numberSecond = InsertInto.Row(second).GetLastInsertIdentity<ulong>().ExecuteRead(Connection).Single();

            Assert.That(numberFirst, Is.EqualTo(1));
            Assert.That(numberSecond, Is.EqualTo(2));

            var firstSelected = Select.From<TableWithIncrement>().WhereEqual(x => x.Identifier, 1UL)
                                      .ExecuteRead(Connection)
                                      .SingleOrDefault();

            Assert.That(firstSelected, Is.Not.Null);
            Assert.That(firstSelected.Identifier, Is.EqualTo(1UL));
            Assert.That(firstSelected.Value, Is.EqualTo("Foo"));
        }

        [Test]
        public void WhenDateOnlyIsInsertedItRoundTrips()
        {
            DropTable.For<DateOnlyTable>().TryExecute(Connection);
            CreateTable.For<DateOnlyTable>().Execute(Connection);

            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            var record = new DateOnlyTable
            {
                Id = 1,
                Value = today
            };

            Assert.That(InsertInto.Row(record).TryExecute(Connection));

            var result = Select.From<DateOnlyTable>()
                .WhereEqual(x => x.Id, 1)
                .ExecuteRead(Connection)
                .SingleOrDefault();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(today));
        }
    }
}
