using System.Collections.Immutable;

namespace Yoeca.Sql
{
    internal sealed class TableDefinition
    {
        public readonly ImmutableList<ColumnRetriever> Columns;

        public readonly Type DataType;

        public readonly string Name;

        public TableDefinition(Type dataType)
        {
            DataType = dataType;

            var definition = dataType.GetCustomAttributes(false).OfType<SqlTableDefinitionAttribute>().Single();

            Name = definition.Name;

            var properties = new List<ColumnRetriever>();

            foreach (var property in dataType.GetProperties())
            {
                ColumnRetriever? column = ColumnRetriever.TryCreate(property);

                if (column != null)
                {
                    properties.Add(column);
                }
            }

            Columns = properties.ToImmutableList();
        }
    }
}