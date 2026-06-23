using System;

namespace TteLcl.Csv2026;

/// <summary>
/// Buffer for one CSV column. This is an abstract base class for typed subclasses
/// that have formatting knowledge. In casu: <see cref="CsvWriteCell{T}"/>.
/// </summary>
public abstract class CsvWriteCell
{
  /// <summary>
  /// Create a new CsvWriteCell
  /// </summary>
  /// <param name="columnName">
  /// The column name
  /// </param>
  protected CsvWriteCell(string columnName)
  {
    ColumnName = columnName;
  }

  /// <summary>
  /// The name of the column
  /// </summary>
  public string ColumnName { get; }

  /// <summary>
  /// The string value of the column. Null indicates the cell is not valid.
  /// Normally set through the Set() method of the typed subclass
  /// </summary>
  public string? Value { get; protected set; }

  /// <summary>
  /// Return <see cref="Value"/> as a CSV safe value. If necessary, the value is quoted
  /// according to CSV quoting rules. If <see cref="Value"/> is null, an exception is thrown.
  /// </summary>
  public string CsvValue {
    get {
      if(Value == null)
      {
        throw new InvalidOperationException(
            $"No value was set for column '{ColumnName}'");
      }
      return CsvFormat.Quote(Value);
    }
  }

  /// <summary>
  /// Clear the <see cref="Value"/> to null, indicating the cell is not valid.
  /// </summary>
  public void Clear()
  {
    Value = null;
  }
}

/// <summary>
/// A stronly typed <see cref="CsvWriteCell"/> subclass, defining a Set() method to
/// set the string value from the typed value.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CsvWriteCell<T>: CsvWriteCell
{
  private readonly Func<T, string> _formatter;

  /// <summary>
  /// A typed subclass with a Set() method that knows how to format a value
  /// of a specific type
  /// </summary>
  /// <param name="columnName"></param>
  /// <param name="formatter"></param>
  public CsvWriteCell(
      string columnName,
      Func<T, string> formatter)
      : base(columnName)
  {
    _formatter = formatter;
  }

  /// <summary>
  /// Set the cell <see cref="CsvWriteCell.Value"/> to a formatted version 
  /// of <paramref name="t"/>
  /// </summary>
  /// <param name="t"></param>
  public void Set(T t)
  {
    Value = _formatter(t);
  }
}
