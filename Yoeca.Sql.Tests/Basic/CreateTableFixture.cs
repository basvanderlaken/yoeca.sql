using System;
using NUnit.Framework;
using Yoeca.Sql.NUnit;

namespace Yoeca.Sql.Tests.Basic
{
    [TestFixture]
    internal sealed class CreateTableFixture
    {
        [Test]
        public void SupportForBasicTypes()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `Extended`(",
                "`Identifier` CHAR(32) NOT NULL, `Name` VARCHAR(128) NOT NULL, `Age` INT SIGNED, `Payload` BLOB NOT NULL,",
                "PRIMARY KEY (`Identifier`)",
                ")");
            var command = CreateTable.For<ExtendedTable>().Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void SupportForDatetime()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `simple_datetime`(",
                "`Value` BIGINT SIGNED",
                ")");
            var command = CreateTable.For<SimpleTableWithDateTime>().Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void SupportForDouble()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `simple_double`(",
                "`Value` DOUBLE",
                ")");
            var command = CreateTable.For<SimpleTableWithDouble>().Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void SupportForDateOnly()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `simple_dateonly`(",
                "`Value` DATE",
                ")");
            var command = CreateTable.For<SimpleTableWithDateOnly>().Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void SupportForTimeOnly()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `simple_timeonly`(",
                "`Value` TIME(3)",
                ")");
            var command = CreateTable.For<SimpleTableWithTimeOnly>().Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void SupportForTimeSpan()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `simple_timespan`(",
                "`Value` TIME(3)",
                ")");
            var command = CreateTable.For<SimpleTableWithTimeSpan>().Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void SupportForEnums()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `enumtable`(",
                "`Name` VARCHAR(128) NOT NULL, `Something` INT SIGNED,",
                "PRIMARY KEY (`Name`)",
                ")");
            var command = CreateTable.For<EnumTable>().Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void SupportForSimpleType()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `Simple`(",
                "`Name` TEXT",
                ")");
            var command = CreateTable.For<SimpleTableWithName>().Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void SupportForNullableDecimal()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `simple_nullable_decimal`(",
                "`Value` DECIMAL(8,2)",
                ")");
            var command = CreateTable.For<SimpleTableWithNullableDecimal>().Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void SupportForBool()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `simple_bool`(",
                "`Value` INT SIGNED",
                ")");
            var command = CreateTable.For<SimpleTableWithBool>().Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void SupportForNullableBool()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `simple_nullable_bool`(",
                "`Value` INT SIGNED",
                ")");
            var command = CreateTable.For<SimpleTableWithNullableBool>().Format(SqlFormat.MySql);

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters, Is.Empty);
        }
    }
}
