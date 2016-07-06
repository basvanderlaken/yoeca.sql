using System;
using NUnit.Framework;
using Yoeca.Sql.NUnit;

namespace Yoeca.Sql.Tests.Basic
{
    [TestFixture]
    internal sealed class SelectFixture
    {
        [Test]
        public void SelectAll()
        {
            Assert.That(Select<SimpleTableWithName>.All().Format(SqlFormat.MySql), Is.EqualTo("SELECT Name FROM Simple"));
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
    }
}