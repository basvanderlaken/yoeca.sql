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

            string expected = @"INSERT INTO Extended (Identifier, Name, Age, Payload) VALUES ('" +
                              value.Identifier.ToString("N") + "', 'Foo', 10, x'FF000000')";


            string command = InsertInto.Row(value).Format(SqlFormat.MySql);

            Assert.That(command, Is.EqualTo(expected));
        }
    }
}