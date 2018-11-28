using System;

namespace Yoeca.Sql
{
    /// <summary>
    /// Attribute to identity a property as a column that should be auto-incremented by the server.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AutoIncrementAttribute : Attribute
    {
    }
}