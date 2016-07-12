using JetBrains.Annotations;

namespace Yoeca.Sql
{
    public interface ISqlCommand<T> : ISqlCommand
    {
        [CanBeNull]
        T TranslateRow([NotNull] ISqlFields fields);
    }

    public interface ISqlCommand
    {
        [NotNull]
        string Format(SqlFormat format);
    }
}