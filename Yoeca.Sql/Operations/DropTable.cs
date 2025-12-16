using System.Linq;

namespace Yoeca.Sql
{
    public sealed class DropTable : ISqlCommand
    {
        public readonly string Name;

        private DropTable(string name)
        {
            Name = name;
        }

        public SqlCommandText Format(SqlFormat format)
        {
            return SqlCommandText.WithoutParameters("DROP TABLE " + SqlIdentifier.Quote(Name, format));
        }

        public static DropTable For<T>()
        {
            var type = typeof(T);

            var definition = type.GetCustomAttributes(false).OfType<SqlTableDefinitionAttribute>().Single();

            return WithName(definition.Name);
        }

        public static DropTable WithName(string name)
        {
            return new DropTable(name);
        }
    }
}
