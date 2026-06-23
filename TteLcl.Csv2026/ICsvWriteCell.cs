using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TteLcl.Csv2026;

/// <summary>
/// Represents a buffer for a single cell in a CSV row (i.e. : a column).
/// This interface is mostly read-only. Setting a value for <see cref="Value"/>
/// is handled in a strongly typed way in the sub-interface <see cref="ICsvWriteCell{T}"/>.
/// </summary>
public interface ICsvWriteCell
{
  /// <summary>
  /// The name of the column
  /// </summary>
  string ColumnName { get; }

  /// <summary>
  /// Get the string value of the column. Null indicates the cell is not valid.
  /// </summary>
  string? Value { get; }

  /// <summary>
  /// Return <see cref="Value"/> as a CSV safe value. If necessary, the value is quoted
  /// according to CSV quoting rules. If <see cref="Value"/> is null, an exception is thrown.
  /// </summary>
  string CsvValue { get; }

  /// <summary>
  /// Clear the <see cref="Value"/> to null, indicating the cell is not valid.
  /// </summary>
  void Clear();
}

/// <summary>
/// Represents a buffer for a single cell in a CSV row (i.e. : a column).
/// This interface augments <see cref="ICsvWriteCell"/> with a way to set
/// <see cref="ICsvWriteCell.Value"/> from a strongly typed source value.
/// </summary>
/// <typeparam name="T">
/// The type used as source for the value.
/// </typeparam>
public interface ICsvWriteCell<in T> : ICsvWriteCell
{
  /// <summary>
  /// Set the cell <see cref="ICsvWriteCell.Value"/> to a formatted version 
  /// of <paramref name="t"/>
  /// </summary>
  /// <param name="t"></param>
  void Set(T t);
}
