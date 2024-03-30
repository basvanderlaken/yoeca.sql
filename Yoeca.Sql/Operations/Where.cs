

namespace Yoeca.Sql
{
    public abstract class Where
    {
        public readonly string Column;

        protected Where(string column)
        {
            Column = column;
        }

        
        public abstract string Format(SqlFormat format);
    }

    public sealed class WhereEqual : Where
    {
        public readonly string Value;

        public WhereEqual( string column,  string value)
            : base(column)
        {
            Value = value;
        }

        public override string Format(SqlFormat format)
        {
            return "WHERE " + Column + " = " + Value;
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

        public override string Format(SqlFormat format)
        {
            return "WHERE " + Column + " <> " + Value;
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

        public override string Format(SqlFormat format)
        {
            return "WHERE " + Column + " LIKE " + Value;
        }
    }
}