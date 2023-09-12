using ToSic.Razor.Blade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// Private Source Code Clean-up Helpers
public class SourceProcessor: Custom.Hybrid.Code14
{
  public string CleanUpSource(string source, string snippetId) {
    source = KeepOnlySnippet(source, snippetId);
    source = ProcessHideTrimSnippet(source);
    var result = SourceTrim(source);
    return result;
  }

  private string KeepOnlySnippet(string source, string id) {
    // Preparations
    if (string.IsNullOrWhiteSpace(id)) return source;
    var idInQuotes = "\"" + id + "\"";

    // V3 New: Ability to auto-find the correct snippet by number
    var patternSnipStartSnipEnd = @"(?:\.SnipStart\(\)+)(?<contents>[\s\S]*?)(?:(@.*\.SnipEnd\(|@Sys\.SourceCode\.Invisible\(\)))"; // note: we're not testing for the final ")"
    var idNumber = Kit.Convert.ToInt(id, fallback: -1);
    if (idNumber >= 0) {
      // V3 with variable (so code doesn't start with @Sys.SourceCode) and SnipStart(...) - and no name!
      var matches = Regex.Matches(source, patternSnipStartSnipEnd);
      if (matches.Count > idNumber) {
        return matches[idNumber].Groups["contents"].Value;
      }
      
    }

    // trim unnecessary comments
    var patternSnippet = @"(?:<snippet id=" + idInQuotes + @"[^>]*>)(?<contents>[\s\S]*?)(?:</snippet>)";
    var match = Regex.Match(source, patternSnippet);
    if (match.Length > 0) {
      return match.Groups["contents"].Value;
    }
    // V2 with Snippet Tabs / Inline Tabs
    var patternStartEnd = @"(?:@Sys\.SourceCode\.(Snippet|Formula)(Inline|Only|Init)?Start\(" + idInQuotes + @"[^\)]*\))(?<contents>[\s\S]*?)(?:@Sys\.SourceCode\.(Snippet|Invisible\())";
    match = Regex.Match(source, patternStartEnd);
    if (match.Length > 0) return match.Groups["contents"].Value;

    // V2 with Result Tabs - for ResultStart(...) and ResultAndSnippetStart
    patternStartEnd = @"(?:@Sys\.SourceCode\.Result[a-zA-Z]*Start\(" + idInQuotes + @"[^\)]*\))(?<contents>[\s\S]*?)(?:@Sys\.SourceCode\.(Result|Invisible))";
    match = Regex.Match(source, patternStartEnd);
    if (match.Length > 0) return match.Groups["contents"].Value;

    // V3 with variable (so code doesn't start with @Sys.SourceCode) and SnipStart(...)
    patternStartEnd = @"(?:\.SnipStart\(" + idInQuotes + @"\)+)(?<contents>[\s\S]*?)(?:@.*\.SnipEnd\(\))";
    match = Regex.Match(source, patternStartEnd);
    if (match.Length > 0) return match.Groups["contents"].Value;

    // V3 with variable (so code doesn't start with @Sys.SourceCode) and SnipStart(...) - and no name!
    match = Regex.Match(source, patternSnipStartSnipEnd);
    if (match.Length > 0) return match.Groups["contents"].Value;

    return source;
  }

  private string ProcessHideTrimSnippet(string source) {
    // trim unnecessary comments
    var patternTrim = @"(?:<trim>)([\s\S]*?)(?:</trim>)";

    source = Regex.Replace(source, patternTrim, m => { 
      var part = Tags.Strip(m.ToString());
      return Text.Ellipsis(part, 40, "... <!-- unimportant stuff, hidden -->");
    });

    // hide unnecessary parts with comment
    var patternHide = @"(?:<hide>)([\s\S]*?)(?:</hide>)";
    source = Regex.Replace(source, patternHide, m => "<!-- unimportant stuff, hidden -->");

    // hide unnecessary parts without comment
    source = ProcessHideSilent(source, "<hide-silent>", "</hide-silent>");
    source = ProcessHideSilent(source, @"@Sys\.SourceCode\.Invisible\(\)", @"@Sys.SourceCode.ResultEnd\(", true, false);

    // remove snippet markers
    var patternSnipStart = @"(?:</?snippet)([\s\S]*?)(?:>)";
    source = Regex.Replace(source, patternSnipStart, "");

    // Remove all @Sys... lines - any whitespace followed by @Sys. till the end of the line
    var patternSysLines = @"^\s*@Sys\.[A-Z].*?$";
    var rxSysLines = new Regex(patternSysLines, RegexOptions.Multiline);
    source = rxSysLines.Replace(source, "");
    return source;
  }

  private string ProcessHideSilent(string source, string start, string end, bool captureStart = true, bool captureEnd = true) {
    var startCapt = captureStart ? ":" : "=";
    var endCapt = captureEnd ? ":" : "=";
    var patternHideSilent = @"(?" + startCapt + start + @")([\s\S]*?)(?" + endCapt + (end ?? start) + @")";
    return Regex.Replace(source, patternHideSilent, "");
  }

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

    var minIndent = indents.Any() ? indents.Min() : 0;

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
}