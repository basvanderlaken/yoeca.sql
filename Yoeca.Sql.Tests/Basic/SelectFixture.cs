using System;
using System.Collections.Immutable;
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
            Assert.That(Select<SimpleTableWithName>.All().Format(SqlFormat.MySql),
                        Is.EqualTo("SELECT Name FROM Simple"));
            Assert.That(Select<ExtendedTable>.All().Format(SqlFormat.MySql),
                        Is.EqualTo("SELECT Identifier, Name, Age, Payload FROM Extended"));
        }

        [Test]
        public void SelectAllWithWhere()
        {
            Assert.That(Select<ExtendedTable>.All().WhereEqual(x => x.Name, "Peter").Format(SqlFormat.MySql),
                        Is.EqualTo("SELECT Identifier, Name, Age, Payload FROM Extended\r\nWHERE Name = 'Peter'"));

            Assert.That(Select<ExtendedTable>.All().WhereNotEqual(x => x.Name, "Peter").Format(SqlFormat.MySql),
                        Is.EqualTo("SELECT Identifier, Name, Age, Payload FROM Extended\r\nWHERE Name <> 'Peter'"));

            Guid identity = Guid.NewGuid();
            string result =
                string.Format("SELECT Identifier, Name, Age, Payload FROM Extended\r\nWHERE Identifier = '{0}'",
                              identity.ToString("N"));
            Assert.That(Select<ExtendedTable>.All().WhereEqual(x => x.Identifier, identity).Format(SqlFormat.MySql),
                        Is.EqualTo(result));
        }

        [Test]
        public void SelectMaximum()
        {
            Assert.That(Select.From<SimpleTableWithDouble>().Maximum(x => x.Value).Format(SqlFormat.MySql),
                        Is.EqualTo("SELECT MAX(Value) FROM simple_double"));
        }

        [Test]
        public void SelectMinimum()
        {
            Assert.That(Select.From<SimpleTableWithDouble>().Minimum(x => x.Value).Format(SqlFormat.MySql),
                        Is.EqualTo("SELECT MIN(Value) FROM simple_double"));
        }

        [Test]
        public void SelectSum()
        {
            Assert.That(Select.From<SimpleTableWithDouble>().Sum(x => x.Value).Format(SqlFormat.MySql),
                        Is.EqualTo("SELECT SUM(Value) FROM simple_double"));
        }

        [Test]
        public void SelectSumWithGrouping()
        {
            Assert.That(Select.From<ExtendedTable>().SumBy(x => x.Age, x => x.Name).Format(SqlFormat.MySql),
                        Is.EqualTo("SELECT Name, SUM(Age) FROM Extended\r\nGROUP BY Name"));
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
