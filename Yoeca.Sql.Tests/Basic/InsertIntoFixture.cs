using System;
using NUnit.Framework;
using Yoeca.Sql.NUnit;

namespace Yoeca.Sql.Tests.Basic
{
    [TestFixture]
    internal sealed class InsertIntoFixture
    {
        [Test]
        public void SupportForBasicTypes()
        {
            var value = new ExtendedTable
            {
                Age = 10,
                Identifier = Guid.NewGuid(),
                Name = "Foo",
                Payload = new SomeOtherClass
                {
                    Content = 255
                }
            };

            string expected = @"INSERT INTO `Extended` (`Identifier`, `Name`, `Age`, `Payload`) VALUES ('" +
                              value.Identifier.ToString("N") + "', 'Foo', 10, x'FF000000')";


            string command = InsertInto.Row(value).Format(SqlFormat.MySql);

            Assert.That(command, Is.EqualTo(expected));

            command = InsertInto.Row(value).UpdateOnDuplicateKey.Format(SqlFormat.MySql);

            string fullExpected = expected + "\r\nON DUPLICATE KEY UPDATE `Identifier`='" +
                                  value.Identifier.ToString("N") +
                                  "', `Name`='Foo', `Age`=10, `Payload`=x'FF000000'";

            Assert.That(command, Is.EqualTo(fullExpected));
        }

        [Test]
        public void SupportForAutoIncrement()
        {
            var value = new TableWithIncrement
            {
                Value = "Foo"
            };

            string expected =
                $"INSERT INTO `with_autoincrement` (`Value`) VALUES ('Foo');{Environment.NewLine}SELECT LAST_INSERT_ID();";
            string command = InsertInto.Row(value).GetLastInsertIdentity<long>().Format(SqlFormat.MySql);

            Assert.That(command, Is.EqualTo(expected));
        }
    }
}
