﻿using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using ProtoBuf;
using Yoeca.Sql.NUnit;

namespace Yoeca.Sql.Tests.Integration
{
    [TestFixture]
    internal sealed class SqlTest
    {
        [Test]
        public void ConnectWithSimpleObject()
        {
            using (var connection = new MySqlConnection(MySqlTestDatabase.ConnectionString))
            {
                connection.Open();

                DropTable.For<Player>().TryExecute(connection);
                CreateTable.For<Player>().Execute(connection);

                var peter = new Player
                {
                    Identifier = Guid.NewGuid(),
                    Name = "Peter",
                    Age = 22,
                    Birthday = new DateTime(1983, 3, 21).ToUniversalTime()
                };

                var willem = new Player
                {
                    Identifier = Guid.NewGuid(),
                    Name = "Willem",
                    Age = 50,
                    Birthday = new DateTime(1983, 3, 22).ToUniversalTime()
                };

                InsertInto.Row(peter).Execute(connection);
                InsertInto.Row(willem).Execute(connection);

                var selectResult = Select.From<Player>().ExecuteRead(connection)
                    .OrderBy(x => x.Name)
                    .ToImmutableList();

                Assert.That(selectResult, Has.Count.EqualTo(2));
                Assert.That(selectResult[0].Name, Is.EqualTo(peter.Name));
                Assert.That(selectResult[0].Identifier, Is.EqualTo(peter.Identifier));
                Assert.That(selectResult[0].Age, Is.EqualTo(peter.Age));
                Assert.That(selectResult[0].Birthday, Is.EqualTo(peter.Birthday));

                Assert.That(selectResult[1].Name, Is.EqualTo(willem.Name));
                Assert.That(selectResult[1].Identifier, Is.EqualTo(willem.Identifier));
                Assert.That(selectResult[1].Age, Is.EqualTo(willem.Age));
                Assert.That(selectResult[1].Birthday, Is.EqualTo(willem.Birthday));


                selectResult = Select.From<Player>()
                    .WhereEqual(x => x.Identifier, peter.Identifier)
                    .ExecuteRead(connection)
                    .OrderBy(x => x.Name).ToImmutableList();

                Assert.That(selectResult, Has.Count.EqualTo(1));
                Assert.That(selectResult[0].Name, Is.EqualTo(peter.Name));

                var maximumAge = Select.From<Player>().Maximum(x => x.Age).ExecuteRead(connection).ToImmutableList();

                Assert.That(maximumAge, Has.Count.EqualTo(1));
                Assert.That(maximumAge[0], Is.EqualTo(50));

                var minimumAge = Select.From<Player>().Minimum(x => x.Age).ExecuteRead(connection).ToImmutableList();

                Assert.That(minimumAge, Has.Count.EqualTo(1));
                Assert.That(minimumAge[0], Is.EqualTo(22));
            }
        }

        [Test]
        public void InsertAndSelectProtoBuffer()
        {
            using (var connection = new MySqlConnection(MySqlTestDatabase.ConnectionString))
            {
                connection.Open();

                DropTable.For<IdentifiedBlob>().TryExecute(connection);
                CreateTable.For<IdentifiedBlob>().Execute(connection);

                var value = new IdentifiedBlob
                {
                    Identifier = Guid.NewGuid(),
                    Value = new Payload
                    {
                        ValueA = 42,
                        ValueB = -44
                    }
                };

                InsertInto.Row(value).Execute(connection);

                var result = Select.From<IdentifiedBlob>().ExecuteRead(connection).ToImmutableList();

                Assert.That(result, Has.Count.EqualTo(1));
                Assert.That(result[0].Identifier, Is.EqualTo(value.Identifier));
                Assert.That(result[0].Value.ValueA, Is.EqualTo(value.Value.ValueA));
                Assert.That(result[0].Value.ValueB, Is.EqualTo(value.Value.ValueB));
            }
        }

        [Test]
        public void VerifyEnumWriteAndRead()
        {
            using (var connection = new MySqlConnection(MySqlTestDatabase.ConnectionString))
            {
                connection.Open();

                DropTable.For<EnumTable>().TryExecute(connection);
                CreateTable.For<EnumTable>().Execute(connection);

                var value = new EnumTable
                {
                    Name = "Foo",
                    Something = Something.Second
                };

                InsertInto.Row(value).Execute(connection);

                var values = Select.From<EnumTable>().WhereEqual(x => x.Name, "Foo").ExecuteRead(connection).ToList();

                Assert.That(values, Has.Count.EqualTo(1));
                Assert.That(values[0].Something, Is.EqualTo(Something.Second));
                Assert.That(values[0].Name, Is.EqualTo("Foo"));
            }
        }

        [Test]
        public void VerifyProtoBuffer()
        {
            byte[] buffer;

            var value = new Payload
            {
                ValueA = 1,
                ValueB = 2
            };

            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, value);

                buffer = stream.ToArray();
            }
            using (var stream = new MemoryStream(buffer))
            {
                var otherValue = Serializer.Deserialize<Payload>(stream);

                Assert.That(otherValue.ValueA, Is.EqualTo(value.ValueA));
                Assert.That(otherValue.ValueB, Is.EqualTo(value.ValueB));
            }
        }
    }
}