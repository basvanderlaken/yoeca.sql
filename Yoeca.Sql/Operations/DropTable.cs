using System.Linq;
using JetBrains.Annotations;

namespace Yoeca.Sql
{
    public sealed class DropTable : ISqlCommand
    {
        public readonly string Name;

        private DropTable([NotNull] string name)
        {
            Name = name;
        }

        public string Format(SqlFormat format)
        {
            return "DROP TABLE " + Name;
        }

        [NotNull]
        public static DropTable For<T>()
        {
            var type = typeof(T);

            var definition = type.GetCustomAttributes(false).OfType<SqlTableDefinitionAttribute>().Single();

            return WithName(definition.Name);
        }

        [NotNull]
        public static DropTable WithName([NotNull] string name)
        {
            return new DropTable(name);
        }
    }
}