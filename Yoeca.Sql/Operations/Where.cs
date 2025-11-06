

namespace Yoeca.Sql
{
    public abstract class Where
    {
        public readonly string Column;

        protected Where(string column)
        {
            Column = column;
        }

        public string Format(SqlFormat format, bool isFirstClause)
        {
            string keyword = isFirstClause ? "WHERE" : "AND";

            return keyword + " " + FormatCondition(format);
        }

        protected abstract string FormatCondition(SqlFormat format);
    }

    public sealed class WhereEqual : Where
    {
        public readonly string Value;

        public WhereEqual( string column,  string value)
            : base(column)
        {
            Value = value;
        }

        protected override string FormatCondition(SqlFormat format)
        {
            return SqlIdentifier.Quote(Column, format) + " = " + Value;
        }
    }

    public sealed class WhereNotEqual : Where
    {
        
        public readonly string Value;

        public WhereNotEqual( string column,  string value)
            : base(column)
        {
            Value = value;
        }

        protected override string FormatCondition(SqlFormat format)
        {
            return SqlIdentifier.Quote(Column, format) + " <> " + Value;
        }
    }

    public sealed class WhereLike : Where
    {
        
        public readonly string Value;

        public WhereLike( string column,  string value)
            : base(column)
        {
            Value = value;
        }

        protected override string FormatCondition(SqlFormat format)
        {
            return SqlIdentifier.Quote(Column, format) + " LIKE " + Value;
        }
    }
}
