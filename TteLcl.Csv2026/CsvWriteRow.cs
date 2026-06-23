using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace TteLcl.Csv2026;

/// <summary>
/// A collection of <see cref="CsvWriteCell"/> instances for buffering CSV content
/// before writing it.
/// </summary>
public class CsvWriteRow
{
  private readonly List<CsvWriteCell> _cells;
  private bool _locked;

  /// <summary>
  /// Create a new, empty, unlocked <see cref="CsvWriteRow"/>.
  /// Call one of the AddXXX methods to add columns (cells).
  /// Call <see cref="EmitHeader"/> to write the header.
  /// Repeatedly Set() each cell to a value and call <see cref="Emit"/> after completing each row.
  /// </summary>
  public CsvWriteRow()
  {
    _cells = new List<CsvWriteCell>();
    Cells = _cells.AsReadOnly();
    _locked = false;
  }

  /// <summary>
  /// Clear all cell values and <see cref="Lock"/> this row.
  /// </summary>
  public void Clear()
  {
    Lock();
    foreach(var cell in _cells)
    {
      cell.Clear();
    }
  }

  /// <summary>
  /// Lock this row and write the header to the <paramref name="output"/>
  /// </summary>
  /// <param name="output"></param>
  public void EmitHeader(TextWriter output)
  {
    Lock();
    for(var i = 0; i < _cells.Count; i++)
    {
      if(i > 0)
      {
        output.Write(',');
      }
      output.Write(CsvFormat.Quote(_cells[i].ColumnName));
    }
    Clear();
    output.WriteLine();
  }

  /// <summary>
  /// Write the current row  to the <paramref name="output"/> and <see cref="Clear"/> this buffer.
  /// If not already done so, this also <see cref="Lock"/>s this buffer first.
  /// </summary>
  /// <param name="output"></param>
  public void Emit(TextWriter output)
  {
    Lock();
    for(var i = 0; i < _cells.Count; i++)
    {
      if(i > 0)
      {
        output.Write(',');
      }
      output.Write(_cells[i].CsvValue);
    }
    Clear();
    output.WriteLine();
  }

  /// <summary>
  /// Lock this row, preventing adding any more columns. Emitting the header
  /// or cell content automatically calls this.
  /// </summary>
  public void Lock()
  {
    if(_cells.Count == 0)
    {
      throw new InvalidOperationException(
          "Attempt to lock down a row before adding any cells to it");
    }
    _locked = true;
  }

  /// <summary>
  /// A read-only view on the ordered list of cells (columns)
  /// </summary>
  public IReadOnlyList<CsvWriteCell> Cells { get; }

  /// <summary>
  /// Return the number of columns (cells) for this row
  /// </summary>
  public int Count => _cells.Count;

  /// <summary>
  /// Add a new <see cref="CsvWriteCell"/> for writing strings.
  /// Set(null) will set the value to an empty string.
  /// </summary>
  public CsvWriteCell<string?> AddStringCell(string columnName)
  {
    return Add(new CsvWriteCell<string?>(
        columnName,
        s => s ?? String.Empty));
  }

  /// <summary>
  /// Add a new <see cref="CsvWriteCell"/> for writing integers.
  /// </summary>
  public CsvWriteCell<long> AddIntegerCell(string columnName)
  {
    return Add(new CsvWriteCell<long>(
        columnName,
        l => l.ToString()));
  }

  /// <summary>
  /// Add a new <see cref="CsvWriteCell"/> for writing floating point values in the default
  /// invariant culture format.
  /// </summary>
  public CsvWriteCell<double> AddFloatingPointCell(string columnName)
  {
    return Add(new CsvWriteCell<double>(
        columnName,
        f => f.ToString(CultureInfo.InvariantCulture)));
  }

  /// <summary>
  /// Add a new <see cref="CsvWriteCell"/> for writing floating point values using
  /// the specified format and invariant culture.
  /// </summary>
  public CsvWriteCell<double> AddFloatingPointCell(string columnName, string format)
  {
    return Add(new CsvWriteCell<double>(
        columnName,
        f => f.ToString(format, CultureInfo.InvariantCulture)));
  }

  /// <summary>
  /// Add a new <see cref="CsvWriteCell"/> for writing boolean values, writing
  /// <paramref name="trueText"/> for true values and <paramref name="falseText"/> for false values.
  /// </summary>
  public CsvWriteCell<bool> AddBooleanCell(string columnName, string trueText, string falseText)
  {
    return Add(new CsvWriteCell<bool>(
        columnName,
        b => b ? trueText : falseText));
  }

  /// <summary>
  /// Add a new <see cref="CsvWriteCell"/> for writing boolean values, writing
  /// "true" for true values and "false" for false values.
  /// </summary>
  public CsvWriteCell<bool> AddBooleanCell(string columnName)
  {
    return AddBooleanCell(columnName, "true", "false");
  }

  /// <summary>
  /// Add a new <see cref="CsvWriteCell"/> for writing enum values.
  /// </summary>
  public CsvWriteCell<TEnum> AddEnumCell<TEnum>(string columnName)
      where TEnum : Enum
  {
    return Add(new CsvWriteCell<TEnum>(
        columnName,
        e => e.ToString()));
  }

  /// <summary>
  /// Add a new <see cref="CsvWriteCell"/> for writing objects using their default
  /// <see cref="object.ToString"/> implementation and converting null values to an empty
  /// string.
  /// </summary>
  public CsvWriteCell<object?> AddObjectCell(string columnName)
  {
    return Add(new CsvWriteCell<object?>(
        columnName,
        o => o == null ? String.Empty : (o.ToString() ?? String.Empty)));
  }

  /// <summary>
  /// Add a prconfigured cell to this <see cref="CsvWriteRow"/>.
  /// </summary>
  /// <param name="cell"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public CsvWriteCell Add(CsvWriteCell cell)
  {
    if(_locked)
    {
      throw new InvalidOperationException(
          "Cannot add any more columns after locking down the row");
    }
    _cells.Add(cell);
    return cell;
  }

  /// <summary>
  /// Add a typed cell
  /// </summary>
  public CsvWriteCell<T> Add<T>(CsvWriteCell<T> cell)
  {
    if(_locked)
    {
      throw new InvalidOperationException(
          "Cannot add any more columns after locking down the row");
    }
    _cells.Add(cell);
    return cell;
  }
}
