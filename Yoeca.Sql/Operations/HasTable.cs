using System;
using System.Linq;

namespace Yoeca.Sql
{
    public sealed class HasTable : ISqlCommand<bool>
    {
        public readonly string Name;

        private HasTable(string name)
        {
            Name = name;
        }

        public bool TranslateRow(ISqlFields fields)
        {
            return fields.Get(0) != null;
        }

        public SqlCommandText Format(SqlFormat format)
        {
            switch (format)
            {
                case SqlFormat.MySql:
                    return SqlCommandText.WithoutParameters(
                        $@"SELECT * FROM information_schema.tables WHERE table_name = '{Name}'  AND table_schema = DATABASE() LIMIT 1");
                default:
                    throw new InvalidOperationException("Unsupported SQL format.");
            }
        }

        public static HasTable For<T>()
        {
            var type = typeof(T);

            var definition = type.GetCustomAttributes(false).OfType<SqlTableDefinitionAttribute>().Single();

            return new HasTable(definition.Name);
        }
    }
}