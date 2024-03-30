
namespace Yoeca.Sql
{
    public interface ISqlCommand<T> : ISqlCommand
    {
        T? TranslateRow(ISqlFields fields);
    }

    public interface ISqlCommand
    {
        string Format(SqlFormat format);
    }
}