using System;

namespace Yoeca.Sql
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FixedSizeAttribute : Attribute
    {
        public readonly int Size;

        public FixedSizeAttribute(int size)
        {
            Size = size;
        }
    }
}