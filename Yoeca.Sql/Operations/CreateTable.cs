using System.Collections.Immutable;
using System.Text;

namespace Yoeca.Sql
{
    public sealed class CreateTable : ISqlCommand
    {
        public readonly ImmutableList<TableColumn> Columns;
        public readonly string Name;

        private CreateTable(string name, ImmutableList<TableColumn> columns)
        {
            Name = name;
            Columns = columns;
        }

        public SqlCommandText Format(SqlFormat format)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("CREATE TABLE {0}", SqlIdentifier.Quote(Name, format));
            builder.AppendLine("(");
            builder.Append(string.Join(", ", Columns.Select(x => x.Format(format))));
            var primaryKeys = Columns.Where(x => x.PrimaryKey).ToList();

            if (primaryKeys.Count > 0)
            {
                builder.AppendLine(",");
                builder.AppendFormat("PRIMARY KEY ({0})",
                                     string.Join(", ", SqlIdentifier.Quote(primaryKeys.Select(x => x.Name), format)));
                builder.AppendLine();
            }
            else
            {
                builder.AppendLine();
            }

            builder.Append(")");

            return SqlCommandText.WithoutParameters(builder.ToString());
        }

        public static CreateTable For<T>()
        {
            var type = typeof(T);

            var definition = type.GetCustomAttributes(false).OfType<SqlTableDefinitionAttribute>().Single();

            var result = WithName(definition.Name);

            foreach (var property in type.GetProperties())
            {
                result = result.With(TableColumn.Create(property));
            }

            return result;
        }

        public static CreateTable WithName( string name)
        {
            return new CreateTable(name, ImmutableList<TableColumn>.Empty);
        }

        public CreateTable With( TableColumn column)
        {
            return new CreateTable(Name, Columns.Add(column));
        }
    }
}
