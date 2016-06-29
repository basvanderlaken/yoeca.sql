using System;

namespace Yoeca.Sql
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MaximumSizeAttribute : Attribute
    {
        public readonly int Size;

        public MaximumSizeAttribute(int size)
        {
            Size = size;
        }
    }
}