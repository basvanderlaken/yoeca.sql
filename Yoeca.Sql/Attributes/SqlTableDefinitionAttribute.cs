using System;

namespace Yoeca.Sql
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SqlTableDefinitionAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
    }
}