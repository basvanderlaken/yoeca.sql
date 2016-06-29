using System;
using JetBrains.Annotations;

namespace Yoeca.Sql
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TableDefinitionAttribute : Attribute
    {
        public readonly string Name;

        public TableDefinitionAttribute([NotNull] string name)
        {
            Name = name;
        }
    }
}