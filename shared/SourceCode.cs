using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

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

  #region Special ShowResult helpers

  public ITag ShowResultJs(string source) { return ShowResult(source, "javascript"); }
  public ITag ShowResultHtml(string source) { return ShowResult(source, "html"); }
  public ITag ShowResultText(string source) { return ShowResult(source, "text"); }
  // Special use case for many picture / image tutorials
  public ITag ShowResultImg(object tag) {
    var cleaned = tag.ToString().Replace(" srcset", " \nsrcset").Replace(",", ",\n");
    return ShowResultHtml(cleaned);
  }

  public ITag ShowResultPic(object tag) {
    var cleaned = tag.ToString()
        .Replace(">", ">\n")
        .Replace(",", ",\n")
        .Replace(" alt=", "\nalt=")
        .Replace("' ", "' \n");
    return ShowResultHtml(cleaned);
  }

  #endregion

  #region Show Source Block

  public dynamic ShowSourceWip(dynamic DynamicModel) {
    // source = SourceTrim(source);
    // size = Size(DynamicModel.Size, source);
    // rndId = Guid.NewGuid().ToString();

    // return Tag.RawHtml(
    //   mainTag,
    //   TurnOnSource(filePath, DynamicModel.Language, wrap, "source" + rndId)
    // );

    return null;
  }



  public dynamic SourceBlock(string source, string snipId, string title, string thingType, bool isExpanded, string domAttribute, int size, string rndId) {

    return Tag.Div().Class("code-block " + (isExpanded ? "is-expanded" : "")).Attr(domAttribute).Wrap(
      snipId == null
        ? Tag.Div().Class("header row justify-content-between").Wrap(
            Tag.Div().Class("col-11").Wrap(
              Tag.H2(title),
              Tag.P("Below you'll see the source code of the " + thingType + @". 
                  Note that we're just showing the main part, and hiding some parts of the file which are not relevant for understanding the essentials. 
                  <strong>Click to expand the code</strong>")
            ),
            Tag.Div().Class("col-auto").Wrap(
              // Up / Down arrows as SVG - hidden by default, become visible based on CSS 
              Tag.Custom("<img src='" + App.Path + "/assets/svg/arrow-up.svg' class='fa-chevron-up'>"),
              Tag.Custom("<img src='" + App.Path + "/assets/svg/arrow-down.svg' class='fa-chevron-down'>")
            )
          ) as ITag
        : Tag.Br(),
      SourceBlockCode(source, size, rndId)
    );
  }


  public ITag ShowResult(string source, string language) {
    source = SourceTrim(source);
    var size = Size(null, source);
    var rndId = Guid.NewGuid().ToString();
    return Tag.Div().Class("pre-result").Wrap(
      SourceBlockCode(source, size, rndId),
      TurnOnSource("", language, false, "source" + rndId)
    );
  }

  public ITag SourceBlockCode(string source, int size, string rndId) {
    return Tag.Div().Class("source-code").Wrap(
      Tag.Pre(Tags.Encode(source)).Id("source" + rndId).Style("height: " + size + "px; font-size: 16px")
    );
  }

  public ITag TurnOnSource(string filePath, string language, bool wrap, string sourceCodeId) {
    language = "ace/mode/" + (language ?? (Text.Has(filePath)
      ? FindAce3LanguageName(filePath)
      : "html"));
    var domAttribute = "source-code-" + CmsContext.Module.Id;

    var turnOnData = new {
      @await = "window.ace",
      run = "window.razorTutorial.initSourceCode()",
      debug = true,
      data = new {
        test = "now-automated",
        domAttribute,
        aceOptions = new {
          wrap,
          language,
          sourceCodeId
        }
      }
    };
    return Tag.Custom("turnOn").Attr("turn-on", turnOnData);
  }


  #endregion

  #region File Processing

  public FileInfo GetFile(string filePath, string file) {
    if (file != null) {
      if (file.IndexOf(".") == -1) {
        file = "_" + file + ".cshtml";
      }
      var lastSlash = filePath.LastIndexOf("/");
      filePath = filePath.Substring(0, lastSlash) + "/" + file;
    }
    var fullPath = filePath;
    if (filePath.IndexOf(":") == -1 && filePath.IndexOf(@"\\") == -1)
      fullPath = GetFullPath(filePath);
    var contents = System.IO.File.ReadAllText(fullPath);
    return new FileInfo { File = file, Path = filePath, FullPath = fullPath, Contents = contents };
  }
  
  public class FileInfo {
    public string File;
    public string Path;
    public string FullPath;
    public string Contents;
  }

  private string GetFullPath(string filePath) {
    #if NETCOREAPP
      // This is the Oqtane implementation - cannot use Server.MapPath
      // 2sxclint:disable:v14-no-getservice
      var hostingEnv = GetService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();
      var pathWithTrimmedFirstSlash = filePath.TrimStart(new [] { '/', '\\' });
      return Path.Combine(hostingEnv.ContentRootPath, pathWithTrimmedFirstSlash);
    #else
      return HttpContext.Current.Server.MapPath(filePath);
    #endif
  }

  #endregion

  #region Source Code Clean-up Helpers

  public string KeepOnlySnippet(string source, string id) {
    if (string.IsNullOrWhiteSpace(id)) return source;
    // trim unnecessary comments
    var patternSnippet = @"(?:<snippet id=""" + id + @"""[^>]*>)(?<contents>[\s\S]*?)(?:</snippet>)";
    var match = Regex.Match(source, patternSnippet);
    if (match.Length > 0) {
      return match.Groups["contents"].Value;
    }
    return source;
  }

  public string ProcessHideTrimSnippet(string source) {
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
    var patternHideSilent = @"(?:<hide-silent>)([\s\S]*?)(?:</hide-silent>)";
    source = Regex.Replace(source, patternHideSilent, "");

    // remove snippet markers
    var patternSnipStart = @"(?:</?snippet)([\s\S]*?)(?:>)";
    source = Regex.Replace(source, patternSnipStart, "");
    return source;
  }
  #endregion
}