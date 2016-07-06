using System;
using JetBrains.Annotations;

namespace Yoeca.Sql
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SqlTableDefinitionAttribute : Attribute
    {
        public readonly string Name;

        public SqlTableDefinitionAttribute([NotNull] string name)
        {
            Name = name;
        }
    }
}