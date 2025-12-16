using System;
using System.Linq;
using NUnit.Framework;
using Yoeca.Sql.NUnit;

namespace Yoeca.Sql.Tests.Basic
{
    [TestFixture]
    internal sealed class UpdateFixture
    {
        [Test]
        public void FormatsUpdateCommand()
        {
            Guid identifier = Guid.Parse("eac14a9332a9480abcf65190c3a2a0d3");

            var command = Update.Table<ExtendedTable>()
                .Set(x => x.Name, "Peter")
                .Set(x => x.Age, 42)
                .WhereEqual(x => x.Identifier, identifier)
                .Format(SqlFormat.MySql);

            string expected =
                "UPDATE `Extended` SET `Name` = 'Peter', `Age` = 42\r\nWHERE `Identifier` = @p0";

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters.Single().Value, Is.EqualTo(identifier.ToString()));
        }

        [Test]
        public void FormatsUpdateCommandWithGreaterOrEqual()
        {
            var command = Update.Table<ExtendedTable>()
                .Set(x => x.Name, "Updated")
                .WhereGreaterOrEqual(x => x.Age, 21)
                .Format(SqlFormat.MySql);

            const string expected =
                "UPDATE `Extended` SET `Name` = 'Updated'\r\nWHERE `Age` >= @p0";

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters.Single().Value, Is.EqualTo("21"));
        }

        [Test]
        public void FormatsUpdateCommandWithLessConstraint()
        {
            var command = Update.Table<ExtendedTable>()
                .Set(x => x.Name, "Updated")
                .WhereLess(x => x.Age, 65)
                .Format(SqlFormat.MySql);

            const string expected =
                "UPDATE `Extended` SET `Name` = 'Updated'\r\nWHERE `Age` < @p0";

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters.Single().Value, Is.EqualTo("65"));
        }

        [Test]
        public void LastSetWins()
        {
            var command = Update.Table<ExtendedTable>()
                .Set(x => x.Name, "First")
                .Set(x => x.Name, "Second")
                .Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo("UPDATE `Extended` SET `Name` = 'Second'"));
            Assert.That(command.Parameters, Is.Empty);
        }
    }
}
