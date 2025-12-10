using System;
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

            string command = Update.Table<ExtendedTable>()
                .Set(x => x.Name, "Peter")
                .Set(x => x.Age, 42)
                .WhereEqual(x => x.Identifier, identifier)
                .Format(SqlFormat.MySql);

            string expected =
                "UPDATE `Extended` SET `Name` = 'Peter', `Age` = 42\r\nWHERE `Identifier` = 'eac14a9332a9480abcf65190c3a2a0d3'";

            Assert.That(command, Is.EqualTo(expected));
        }

        [Test]
        public void FormatsUpdateCommandWithGreaterOrEqual()
        {
            string command = Update.Table<ExtendedTable>()
                .Set(x => x.Name, "Updated")
                .WhereGreaterOrEqual(x => x.Age, 21)
                .Format(SqlFormat.MySql);

            const string expected =
                "UPDATE `Extended` SET `Name` = 'Updated'\r\nWHERE `Age` >= 21";

            Assert.That(command, Is.EqualTo(expected));
        }

        [Test]
        public void FormatsUpdateCommandWithLessConstraint()
        {
            string command = Update.Table<ExtendedTable>()
                .Set(x => x.Name, "Updated")
                .WhereLess(x => x.Age, 65)
                .Format(SqlFormat.MySql);

            const string expected =
                "UPDATE `Extended` SET `Name` = 'Updated'\r\nWHERE `Age` < 65";

            Assert.That(command, Is.EqualTo(expected));
        }

        [Test]
        public void LastSetWins()
        {
            string command = Update.Table<ExtendedTable>()
                .Set(x => x.Name, "First")
                .Set(x => x.Name, "Second")
                .Format(SqlFormat.MySql);

            Assert.That(command, Is.EqualTo("UPDATE `Extended` SET `Name` = 'Second'"));
        }
    }
}
