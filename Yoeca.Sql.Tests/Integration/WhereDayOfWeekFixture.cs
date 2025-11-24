using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Yoeca.Sql.Tests.Integration
{
    [TestFixture]
    internal sealed class WhereDayOfWeekFixture : SqlBaseFixture
    {
        [Test]
        public async Task FiltersOnDayOfWeekForDateOnlyColumn()
        {
            DropTable.For<DateOnlyTable>().TryExecute(Connection);
            await Table.Ensure<DateOnlyTable>(Connection);

            var monday = new DateOnly(2024, 4, 1); // Monday
            var tuesday = monday.AddDays(1);
            var sunday = monday.AddDays(-1);

            await InsertInto.Row(new DateOnlyTable { Id = 1, Value = monday }).ExecuteAsync(Connection);
            await InsertInto.Row(new DateOnlyTable { Id = 2, Value = tuesday }).ExecuteAsync(Connection);
            await InsertInto.Row(new DateOnlyTable { Id = 3, Value = sunday }).ExecuteAsync(Connection);

            var mondayRows = Select.From<DateOnlyTable>()
                .WhereDayOfWeek(x => x.Value, DayOfWeek.Monday)
                .ExecuteRead(Connection)
                .ToList();

            Assert.That(mondayRows, Has.Count.EqualTo(1));
            Assert.That(mondayRows[0].Id, Is.EqualTo(1));

            var sundayRows = Select.From<DateOnlyTable>()
                .WhereDayOfWeek(x => x.Value, DayOfWeek.Sunday)
                .ExecuteRead(Connection)
                .ToList();

            Assert.That(sundayRows, Has.Count.EqualTo(1));
            Assert.That(sundayRows[0].Id, Is.EqualTo(3));
        }
    }
}
