using System;
using System.Linq;
using NUnit.Framework;

namespace Yoeca.Sql.Tests.Integration
{
    [TestFixture]
    internal sealed class WhereComparisonFixture : SqlBaseFixture
    {
        [Test]
        public void WhereGreaterOrEqualFiltersResults()
        {
            DropTable.For<Player>().TryExecute(Connection);
            CreateTable.For<Player>().Execute(Connection);

            var younger = new Player
            {
                Identifier = Guid.NewGuid(),
                Name = "Younger",
                Age = 17,
                Birthday = new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            var adult = new Player
            {
                Identifier = Guid.NewGuid(),
                Name = "Adult",
                Age = 21,
                Birthday = new DateTime(2004, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            var older = new Player
            {
                Identifier = Guid.NewGuid(),
                Name = "Older",
                Age = 35,
                Birthday = new DateTime(1989, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            InsertInto.Row(younger).Execute(Connection);
            InsertInto.Row(adult).Execute(Connection);
            InsertInto.Row(older).Execute(Connection);

            var results = Select.From<Player>()
                .WhereGreaterOrEqual(x => x.Age, 21)
                .ExecuteRead(Connection)
                .OrderBy(x => x.Age)
                .ToList();

            Assert.That(results, Has.Count.EqualTo(2));
            Assert.That(results[0].Name, Is.EqualTo(adult.Name));
            Assert.That(results[1].Name, Is.EqualTo(older.Name));

            var minors = Select.From<Player>()
                .WhereLess(x => x.Age, 21)
                .ExecuteRead(Connection)
                .ToList();

            Assert.That(minors, Has.Count.EqualTo(1));
            Assert.That(minors[0].Name, Is.EqualTo(younger.Name));
        }
    }
}
