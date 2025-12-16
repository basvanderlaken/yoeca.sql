
namespace Yoeca.Sql
{
    public abstract class NoParameterSqlCommand : ISqlCommand
    {
        public SqlCommandText Format(SqlFormat format) => new SqlCommandText(FormatWithoutParameters(format), []);

        public abstract string FormatWithoutParameters(SqlFormat format);
    }
}
