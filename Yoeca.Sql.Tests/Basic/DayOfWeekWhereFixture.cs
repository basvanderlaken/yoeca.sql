using System;
using NUnit.Framework;

namespace Yoeca.Sql.Tests.Basic
{
    [TestFixture]
    internal sealed class DayOfWeekWhereFixture
    {
        [Test]
        public void WhereDayOfWeekFormatsUsingMySqlFunction()
        {
            string expected = string.Join(
                Environment.NewLine,
                "SELECT `Value` FROM `simple_dateonly`",
                "WHERE DAYOFWEEK(`Value`) = 2");

            var command = Select.From<SimpleTableWithDateOnly>()
                .WhereDayOfWeek(x => x.Value, DayOfWeek.Monday)
                .Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters, Is.Empty);
        }
    }
}
