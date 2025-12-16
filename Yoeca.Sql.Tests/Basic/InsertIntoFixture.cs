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


            var command = InsertInto.Row(value).Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters, Is.Empty);

            command = InsertInto.Row(value).UpdateOnDuplicateKey.Format(SqlFormat.MySql);

            string fullExpected = expected + "\r\nON DUPLICATE KEY UPDATE `Identifier`='" +
                                  value.Identifier.ToString("N") +
                                  "', `Name`='Foo', `Age`=10, `Payload`=x'FF000000'";

            Assert.That(command.Command, Is.EqualTo(fullExpected));
            Assert.That(command.Parameters, Is.Empty);
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
            var command = InsertInto.Row(value).GetLastInsertIdentity<long>().Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void SupportsNullableGuidFormatting()
        {
            var value = new Yoeca.Sql.Tests.Integration.NullableGuidTable
            {
                Id = 5,
                OptionalIdentifier = null,
            };

            var command = InsertInto.Row(value).Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo("INSERT INTO `nullable_guid` (`Id`, `OptionalIdentifier`) VALUES (5, NULL)"));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void TranslateRowConvertsProviderValueToRequestedType()
        {
            var insert = InsertInto.Row(new TableWithIncrement { Value = "Foo" }).GetLastInsertIdentity<int>();
            var translated = insert.TranslateRow(new StubFields(5L));

            Assert.That(translated, Is.EqualTo(5));
        }

        private sealed class StubFields : ISqlFields
        {
            private readonly object mValue;

            public StubFields(object value)
            {
                mValue = value;
            }

            public object Get(int fieldIndex)
            {
                return mValue;
            }
        }
    }
}
