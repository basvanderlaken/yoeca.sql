using System;
using System.Collections.Immutable;
using System.Linq;

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

        public abstract ImmutableArray<SqlParameterValue> Parameters
        {
            get;
        }
    }

    public sealed class WhereEqual : Where
    {
        public WhereEqual(string column, string parameterName, object? value)
            : base(column)
        {
            ParameterName = parameterName;
            Value = value;
        }

        public string ParameterName
        {
            get;
        }

        public object? Value
        {
            get;
        }

        public override ImmutableArray<SqlParameterValue> Parameters => ImmutableArray.Create(new SqlParameterValue(ParameterName, Value));

        protected override string FormatCondition(SqlFormat format)
        {
            return SqlIdentifier.Quote(Column, format) + " = " + ParameterName;
        }
    }

    public sealed class WhereGreaterOrEqual : Where
    {
        public WhereGreaterOrEqual(string column, string parameterName, object? value)
            : base(column)
        {
            ParameterName = parameterName;
            Value = value;
        }

        public string ParameterName
        {
            get;
        }

        public object? Value
        {
            get;
        }

        public override ImmutableArray<SqlParameterValue> Parameters => ImmutableArray.Create(new SqlParameterValue(ParameterName, Value));

        protected override string FormatCondition(SqlFormat format)
        {
            return SqlIdentifier.Quote(Column, format) + " >= " + ParameterName;
        }
    }

    public sealed class WhereLess : Where
    {
        public WhereLess(string column, string parameterName, object? value)
            : base(column)
        {
            ParameterName = parameterName;
            Value = value;
        }

        public string ParameterName
        {
            get;
        }

        public object? Value
        {
            get;
        }

        public override ImmutableArray<SqlParameterValue> Parameters => ImmutableArray.Create(new SqlParameterValue(ParameterName, Value));

        protected override string FormatCondition(SqlFormat format)
        {
            return SqlIdentifier.Quote(Column, format) + " < " + ParameterName;
        }
    }

    public sealed class WhereNotEqual : Where
    {
        public WhereNotEqual(string column, string parameterName, object? value)
            : base(column)
        {
            ParameterName = parameterName;
            Value = value;
        }

        public string ParameterName
        {
            get;
        }

        public object? Value
        {
            get;
        }

        public override ImmutableArray<SqlParameterValue> Parameters => ImmutableArray.Create(new SqlParameterValue(ParameterName, Value));

        protected override string FormatCondition(SqlFormat format)
        {
            return SqlIdentifier.Quote(Column, format) + " <> " + ParameterName;
        }
    }

    public sealed class WhereLike : Where
    {
        public WhereLike(string column, string parameterName, object? value)
            : base(column)
        {
            ParameterName = parameterName;
            Value = value;
        }

        public string ParameterName
        {
            get;
        }

        public object? Value
        {
            get;
        }

        public override ImmutableArray<SqlParameterValue> Parameters => ImmutableArray.Create(new SqlParameterValue(ParameterName, Value));

        protected override string FormatCondition(SqlFormat format)
        {
            return SqlIdentifier.Quote(Column, format) + " LIKE " + ParameterName;
        }
    }

    public sealed class WhereDayOfWeek : Where
    {
        public readonly int DayOfWeek;

        public WhereDayOfWeek(string column, int dayOfWeek)
            : base(column)
        {
            DayOfWeek = dayOfWeek;
        }

        protected override string FormatCondition(SqlFormat format)
        {
            return $"DAYOFWEEK({SqlIdentifier.Quote(Column, format)}) = {DayOfWeek}";
        }

        public override ImmutableArray<SqlParameterValue> Parameters => ImmutableArray<SqlParameterValue>.Empty;
    }

    public sealed class WhereIn : Where
    {
        public WhereIn(string column, ImmutableArray<string> parameterNames, ImmutableArray<object?> values)
            : base(column)
        {
            ParameterNames = parameterNames;
            Values = values;
        }

        public ImmutableArray<string> ParameterNames
        {
            get;
        }

        public ImmutableArray<object?> Values
        {
            get;
        }

        public override ImmutableArray<SqlParameterValue> Parameters =>
            ParameterNames.Zip(Values, (name, value) => new SqlParameterValue(name, value)).ToImmutableArray();

        protected override string FormatCondition(SqlFormat format)
        {
            return $"{SqlIdentifier.Quote(Column, format)} IN ({string.Join(", ", ParameterNames)})";
        }
    }
}
