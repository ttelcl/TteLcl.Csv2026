# TteLcl.Csv2026

Yet another library to support CSV serialization.

Right now this only supports writing CSV.

## Example

```cs
// Create a CSV row buffer
var row = new CsvWriteRow();

// Declare the columns to write
var colId = row.AddIntegerCell("id");
var colName = row.AddStringCell("name");
var colFlag = row.AddBooleanCell("flag");

// Emit the header (and lock down the set of columns)
row.EmitHeader(writer);

// Emit the data rows
foreach (var datarow in data) {
  colId.Set(datarow.Id);
  colName.Set(datarow.Name);
  colFlag.Set(datarow.Flag);
  row.Emit(writer);
}

```
