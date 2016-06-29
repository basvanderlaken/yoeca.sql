using JetBrains.Annotations;

namespace Yoeca.Sql
{
    public interface ISqlFields
    {
        [CanBeNull]
        object Get(int fieldIndex);
    }
}