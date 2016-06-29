using System;

namespace Yoeca.Sql
{
    /// <summary>
    /// Attribute to identify a property as a primary key of a table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SqlPrimaryKeyAttribute : Attribute
    {
    }
}