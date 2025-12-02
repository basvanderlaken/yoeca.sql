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
            string command = CreateTable.For<ExtendedTable>().Format(SqlFormat.MySql);

            Assert.That(command, Is.EqualTo(expected));
        }

        [Test]
        public void SupportForDatetime()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `simple_datetime`(",
                "`Value` BIGINT SIGNED",
                ")");
            string command = CreateTable.For<SimpleTableWithDateTime>().Format(SqlFormat.MySql);

            Assert.That(command, Is.EqualTo(expected));
        }

        [Test]
        public void SupportForDouble()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `simple_double`(",
                "`Value` DOUBLE",
                ")");
            string command = CreateTable.For<SimpleTableWithDouble>().Format(SqlFormat.MySql);

            Assert.That(command, Is.EqualTo(expected));
        }

        [Test]
        public void SupportForDateOnly()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `simple_dateonly`(",
                "`Value` DATE",
                ")");
            string command = CreateTable.For<SimpleTableWithDateOnly>().Format(SqlFormat.MySql);

            Assert.That(command, Is.EqualTo(expected));
        }

        [Test]
        public void SupportForTimeOnly()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `simple_timeonly`(",
                "`Value` TIME(3)",
                ")");
            string command = CreateTable.For<SimpleTableWithTimeOnly>().Format(SqlFormat.MySql);

            Assert.That(command, Is.EqualTo(expected));
        }

        [Test]
        public void SupportForTimeSpan()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `simple_timespan`(",
                "`Value` TIME(3)",
                ")");
            string command = CreateTable.For<SimpleTableWithTimeSpan>().Format(SqlFormat.MySql);

            Assert.That(command, Is.EqualTo(expected));
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
            string command = CreateTable.For<EnumTable>().Format(SqlFormat.MySql);

            Assert.That(command, Is.EqualTo(expected));
        }

        [Test]
        public void SupportForSimpleType()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `Simple`(",
                "`Name` TEXT",
                ")");
            string command = CreateTable.For<SimpleTableWithName>().Format(SqlFormat.MySql);

            Assert.That(command, Is.EqualTo(expected));
        }

        [Test]
        public void SupportForNullableDecimal()
        {
            string expected = string.Join(
                Environment.NewLine,
                "CREATE TABLE `simple_nullable_decimal`(",
                "`Value` DECIMAL(8,2)",
                ")");
            string command = CreateTable.For<SimpleTableWithNullableDecimal>().Format(SqlFormat.MySql);

            Assert.That(command, Is.EqualTo(expected));
        }
    }
}
