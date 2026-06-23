using System;
using System.Text;

namespace TteLcl.Csv2026;

internal static class CsvFormat
{

  /// <summary>
  /// If <paramref name="value"/> requires quoting to be used in a CSV file, return the
  /// quoted value. Otherwise return <paramref name="value"/> itself.
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  internal static string Quote(string value)
  {
    if(string.IsNullOrEmpty(value))
    {
      // Purposes:
      // (1) Make sure we can index the first and last character in code not trapped here
      // (2) Translate null to an empty string
      return string.Empty;
    }
    if(value.IndexOfAny(",\r\n\"".ToCharArray()) >= 0
        || char.IsWhiteSpace(value[0])
        || char.IsWhiteSpace(value[value.Length - 1]))
    {
      var sb = new StringBuilder();
      sb.Append('"');
      foreach(var ch in value)
      {
        if(ch == '"')
        {
          // Duplicate the '"'
          sb.Append('"');
          sb.Append('"');
        }
        else
        {
          sb.Append(ch);
        }
      }
      sb.Append('"');
      return sb.ToString();
    }
    else
    {
      return value;
    }
  }
}
