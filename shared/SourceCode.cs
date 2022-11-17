using ToSic.Razor.Blade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class SourceCode: Custom.Hybrid.Code14
{
  const int LineHeightPx = 20;
  const int BufferHeightPx = 20; // for footer scrollbar which often appears

  public string SourceTrim(string source) {
    // optimize to remove leading or trailing (but not in the middle)
    var lines = Regex.Split(source ?? "", "\r\n|\r|\n").ToList();
    var result = DropLeadingEmpty(lines);
    result.Reverse();
    result = DropLeadingEmpty(result);
    result.Reverse();

    // Count trailing spaces on all code, to see if all have the same indent
    var indents = result
      .Where(line => !string.IsNullOrWhiteSpace(line))
      .Select(line => line.TakeWhile(Char.IsWhiteSpace).Count());

    var minIndent = indents.Min();

    result = result
      .Select(line => string.IsNullOrWhiteSpace(line) ? line : line.Substring(minIndent))
      .ToList();

    // result.Add("Debug: indent =" + minIndent);
    return string.Join("\n", result);
  }

  private List<string> DropLeadingEmpty(List<string> lines) {
    var dropEmpty = true;
    return lines.Select(l => {
      if (!dropEmpty) return l;
      if (l.Trim() == "") return null;
      dropEmpty = false;
      return l;
    })
    .Where(l => l != null)
    .ToList();
  }

  // Auto-calculate Size
  public int Size(object sizeObj, string source) {
    var size = Kit.Convert.ToInt(sizeObj, fallback: -1);
    if (size == -1) {
      var sourceLines = source.Split('\n').Length;
      size = sourceLines * LineHeightPx + BufferHeightPx;
    }

    if (size < LineHeightPx) size = 600;
    return size;
  }

  // Determine the ace9 language of the file
  public string FindAce3LanguageName(string filePath) {
    var extension = filePath.Substring(filePath.LastIndexOf('.') + 1);
    switch (extension)
    {
      case "cs": return "csharp";
      case "js": return "javascript";
      case "json": return "json";
      default: return "razor";
    }
  }
}