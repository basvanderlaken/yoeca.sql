using System;
using System.Linq;
using JetBrains.Annotations;

namespace Yoeca.Sql
{
    public sealed class HasTable : ISqlCommand<bool>
    {
        [NotNull]
        public readonly string Name;

        private HasTable([NotNull] string name)
        {
            Name = name;
        }

        public bool TranslateRow(ISqlFields fields)
        {
            return fields.Get(0) != null;
        }

        public string Format(SqlFormat format)
        {
            switch (format)
            {
                case SqlFormat.MySql:
                    return
                        $@"SELECT * FROM information_schema.tables WHERE table_name = '{Name}' LIMIT 1";
                default:
                    throw new InvalidOperationException("Unsupported SQL format.");
            }
        }

        [NotNull]
        public static HasTable For<T>()
        {
            var type = typeof(T);

            var definition = type.GetCustomAttributes(false).OfType<SqlTableDefinitionAttribute>().Single();

            return new HasTable(definition.Name);
        }
    }
}