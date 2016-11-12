using JetBrains.Annotations;

namespace Yoeca.Sql
{
    public abstract class Where
    {
        public readonly string Column;

        protected Where(string column)
        {
            Column = column;
        }

        [NotNull]
        public abstract string Format(SqlFormat format);
    }

    public sealed class WhereEqual : Where
    {
        public readonly string Value;

        public WhereEqual([NotNull] string column, [NotNull] string value)
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
        [NotNull]
        public readonly string Value;

        public WhereNotEqual([NotNull] string column, [NotNull] string value)
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
        [NotNull]
        public readonly string Value;

        public WhereLike([NotNull] string column, [NotNull] string value)
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