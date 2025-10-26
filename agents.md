# Yoeca.Sql Agents Notes

## Aggregate helpers
- `Select.Sum` emits a single aggregated scalar for a column.
- `Select.SumBy` returns grouped results through `SelectGroupedValue`.
- `GroupedValue<TGroup, TValue>` pairs the group key with its aggregated value.

## Documentation contract
- New or modified code in this repository must include Microsoft-style XML documentation to keep builds clean (warnings are treated as errors).

## Testing hints
- Run `dotnet test Yoeca.Sql.Tests` from the `yoeca.sql` folder to cover both basic and integration fixtures.
