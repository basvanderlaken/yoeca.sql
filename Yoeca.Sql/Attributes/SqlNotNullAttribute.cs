using System;

namespace Yoeca.Sql
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SqlNotNullAttribute : Attribute
    {
    }
}