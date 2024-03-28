using ToSic.Razor.Blade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AppCode.TutorialSystem.Source
{
  // Private Source Code Clean-up Helpers
  public class SourceProcessor: AppCode.Services.ServiceBase
  {
    public string CleanUpSource(string source) { // , string snippetId) {
      // source = KeepOnlySnippet(source, snippetId);
      source = ProcessHideTrimSnippet(source);
      var result = SourceTrim(source);
      return result;
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
      // hide things between @*! and !*@ - usually used for hiding parts of code
      source = ProcessHideSilent(source, "@\\*!", "!\\*@");
      // hide things between @{/*! and !*/} - usually used for hiding parts of code
      // Usually @{/*!*/ ... /*!*/}
      source = ProcessHideSilent(source, @"@{/\*!", @"!\*/}"); 
      // source = ProcessHideSilent(source, @"@Sys\.SourceCode\.Invisible\(\)", @"@Sys.SourceCode.ResultEnd\(", true, false);

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
      var l = Log.Call<string>();
      var startCapt = captureStart ? ":" : "=";
      var endCapt = captureEnd ? ":" : "=";
      var patternHideSilent = @"(?" + startCapt + start + @")([\s\S]*?)(?" + endCapt + (end ?? start) + @")";
      Log.Add("Pattern: " + patternHideSilent);
      var result = Regex.Replace(source, patternHideSilent, "");
      return l(result, "changed: " + (result != source));
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
}