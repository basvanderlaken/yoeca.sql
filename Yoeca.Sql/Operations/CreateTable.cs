using System.Collections.Immutable;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Yoeca.Sql
{
    public sealed class CreateTable : ISqlCommand
    {
        [NotNull]
        public readonly ImmutableList<TableColumn> Columns;

        [NotNull]
        public readonly string Name;

        private CreateTable([NotNull] string name, [NotNull] ImmutableList<TableColumn> columns)
        {
            Name = name;
            Columns = columns;
        }

        public string Format(SqlFormat format)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("CREATE TABLE {0}", Name);
            builder.AppendLine("(");
            builder.Append(string.Join(", ", Columns.Select(x => x.Format(format))));
            var primaryKeys = Columns.Where(x => x.PrimaryKey).ToList();

            if (primaryKeys.Count > 0)
            {
                builder.AppendLine(",");
                builder.AppendFormat("PRIMARY KEY ({0})", string.Join(", ", primaryKeys.Select(x => x.Name)));
                builder.AppendLine();
            }
            else
            {
                builder.AppendLine();
            }

            builder.Append(")");

            return builder.ToString();
        }

        [NotNull]
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

        [NotNull]
        public static CreateTable WithName([NotNull] string name)
        {
            return new CreateTable(name, ImmutableList<TableColumn>.Empty);
        }

        [NotNull]
        public CreateTable With([NotNull] TableColumn column)
        {
            return new CreateTable(Name, Columns.Add(column));
        }
    }
}