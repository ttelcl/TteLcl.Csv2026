using System;

namespace TteLcl.Csv2026;

/// <summary>
/// Buffer for one CSV column. This is an abstract base class for typed subclasses
/// that have formatting knowledge. In casu: <see cref="CsvWriteCell{T}"/>.
/// </summary>
public abstract class CsvWriteCell : ICsvWriteCell
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

  /// <inheritdoc/>
  public string ColumnName { get; }

  /// <summary>
  /// <inheritdoc/>
  /// This implementation adds a protected setter on top of what the interface requires.
  /// </summary>
  public string? Value { get; protected set; }

  /// <inheritdoc/>
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

  /// <inheritdoc/>
  public void Clear()
  {
    Value = null;
  }
}

/// <summary>
/// A strongly typed <see cref="CsvWriteCell"/> subclass, defining a Set() method to
/// set the string value from the typed value.
/// </summary>
/// <typeparam name="T">
/// The type used as source for the value.
/// </typeparam>
public class CsvWriteCell<T>: CsvWriteCell, ICsvWriteCell<T>
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

  /// <inheritdoc/>
  public void Set(T t)
  {
    Value = _formatter(t);
  }
}
