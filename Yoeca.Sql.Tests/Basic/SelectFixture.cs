using System;
using System.Collections.Immutable;
using System.Linq;
using NUnit.Framework;
using Yoeca.Sql.NUnit;

#nullable enable

namespace Yoeca.Sql.Tests.Basic
{
    [TestFixture]
    internal sealed class SelectFixture
    {
        [Test]
        public void SelectAll()
        {
            var simple = Select<SimpleTableWithName>.All().Format(SqlFormat.MySql);
            Assert.That(simple.Command, Is.EqualTo("SELECT `Name` FROM `Simple`"));
            Assert.That(simple.Parameters, Is.Empty);

            var extended = Select<ExtendedTable>.All().Format(SqlFormat.MySql);
            Assert.That(extended.Command, Is.EqualTo("SELECT `Identifier`, `Name`, `Age`, `Payload` FROM `Extended`"));
            Assert.That(extended.Parameters, Is.Empty);
        }

        [Test]
        public void SelectAllWithWhere()
        {
            var nameEquality = Select<ExtendedTable>.All().WhereEqual(x => x.Name, "Peter").Format(SqlFormat.MySql);
            Assert.That(nameEquality.Command,
                        Is.EqualTo("SELECT `Identifier`, `Name`, `Age`, `Payload` FROM `Extended`\r\nWHERE `Name` = @p0"));
            Assert.That(nameEquality.Parameters.Single().Value, Is.EqualTo("Peter"));

            var nameInequality = Select<ExtendedTable>.All().WhereNotEqual(x => x.Name, "Peter").Format(SqlFormat.MySql);
            Assert.That(nameInequality.Command,
                        Is.EqualTo("SELECT `Identifier`, `Name`, `Age`, `Payload` FROM `Extended`\r\nWHERE `Name` <> @p0"));
            Assert.That(nameInequality.Parameters.Single().Value, Is.EqualTo("Peter"));

            Guid identity = Guid.NewGuid();
            var identityFilter = Select<ExtendedTable>.All().WhereEqual(x => x.Identifier, identity).Format(SqlFormat.MySql);
            const string result = "SELECT `Identifier`, `Name`, `Age`, `Payload` FROM `Extended`\r\nWHERE `Identifier` = @p0";
            Assert.That(identityFilter.Command, Is.EqualTo(result));
            Assert.That(identityFilter.Parameters.Single().Value, Is.EqualTo(identity.ToString("N")));
        }

        [Test]
        public void SelectAllWithMultipleWhereClauses()
        {
            var command = Select<ExtendedTable>.All()
                .WhereEqual(x => x.Name, "Peter")
                .WhereEqual(x => x.Age, 42)
                .Format(SqlFormat.MySql);

            const string expected =
                "SELECT `Identifier`, `Name`, `Age`, `Payload` FROM `Extended`\r\nWHERE `Name` = @p0\r\nAND `Age` = @p1";

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters.Select(x => x.Name), Is.EqualTo(new[] { "@p0", "@p1" }));
            Assert.That(command.Parameters.Select(x => x.Value), Is.EqualTo(new object?[] { "Peter", "42" }));
        }

        [Test]
        public void SelectWithGreaterOrEqualWhereClause()
        {
            var command = Select<ExtendedTable>.All()
                .WhereGreaterOrEqual(x => x.Age, 18)
                .Format(SqlFormat.MySql);

            const string expected =
                "SELECT `Identifier`, `Name`, `Age`, `Payload` FROM `Extended`\r\nWHERE `Age` >= @p0";

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters.Single().Value, Is.EqualTo("18"));
        }

        [Test]
        public void SelectWithLessWhereClause()
        {
            var command = Select<ExtendedTable>.All()
                .WhereLess(x => x.Age, 18)
                .Format(SqlFormat.MySql);

            const string expected =
                "SELECT `Identifier`, `Name`, `Age`, `Payload` FROM `Extended`\r\nWHERE `Age` < @p0";

            Assert.That(command.Command, Is.EqualTo(expected));
            Assert.That(command.Parameters.Single().Value, Is.EqualTo("18"));
        }

        [Test]
        public void SelectMaximum()
        {
            var command = Select.From<SimpleTableWithDouble>().Maximum(x => x.Value).Format(SqlFormat.MySql);
            Assert.That(command.Command, Is.EqualTo("SELECT MAX(`Value`) FROM `simple_double`"));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void SelectMinimum()
        {
            var command = Select.From<SimpleTableWithDouble>().Minimum(x => x.Value).Format(SqlFormat.MySql);
            Assert.That(command.Command, Is.EqualTo("SELECT MIN(`Value`) FROM `simple_double`"));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void SelectSum()
        {
            var command = Select.From<SimpleTableWithDouble>().Sum(x => x.Value).Format(SqlFormat.MySql);
            Assert.That(command.Command, Is.EqualTo("SELECT SUM(`Value`) FROM `simple_double`"));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void SelectSumWithGrouping()
        {
            var command = Select.From<ExtendedTable>().SumBy(x => x.Age, x => x.Name).Format(SqlFormat.MySql);
            Assert.That(command.Command, Is.EqualTo("SELECT `Name`, SUM(`Age`) FROM `Extended`\r\nGROUP BY `Name`"));
            Assert.That(command.Parameters, Is.Empty);
        }

        [Test]
        public void SelectValueTranslatesConvertibleTypes()
        {
            var command = new SelectValue<SimpleTableWithDouble, int>(
                "simple_double",
                "Value",
                ImmutableList<Where>.Empty,
                ValueOperations.Sum);

            Assert.That(command.TranslateRow(new FakeFields(12L)), Is.EqualTo(12));
        }

        [Test]
        public void SelectGroupedValueTranslatesConvertibleTypes()
        {
            var command = new SelectGroupedValue<ExtendedTable, string, int>(
                "Extended",
                "Name",
                "Age",
                ImmutableList<Where>.Empty);

            var result = command.TranslateRow(new FakeFields("Peter", 12L));

            Assert.That(result.Group, Is.EqualTo("Peter"));
            Assert.That(result.Value, Is.EqualTo(12));
        }

        private sealed class FakeFields : ISqlFields
        {
            private readonly object?[] mValues;

            public FakeFields(params object?[] values)
            {
                mValues = values;
            }

            public object? Get(int fieldIndex)
            {
                return mValues[fieldIndex];
            }
        }
    }
}
