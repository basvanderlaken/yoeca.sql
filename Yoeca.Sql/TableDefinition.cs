using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

namespace Yoeca.Sql
{
    internal sealed class TableDefinition
    {
        [NotNull, ItemNotNull]
        public readonly ImmutableList<ColumnRetriever> Columns;

        [NotNull]
        public readonly Type DataType;

        [NotNull]
        public readonly string Name;

        public TableDefinition([NotNull] Type dataType)
        {
            DataType = dataType;

            var definition = dataType.GetCustomAttributes(false).OfType<SqlTableDefinitionAttribute>().Single();

            Name = definition.Name;

            var properties = new List<ColumnRetriever>();

            foreach (var property in dataType.GetProperties())
            {
                ColumnRetriever column = ColumnRetriever.TryCreate(property);

                if (column != null)
                {
                    properties.Add(column);
                }
            }

            Columns = properties.ToImmutableList();
        }
    }
}