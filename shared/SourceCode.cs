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

  public SourceCode Init(string path) {
    Path = path;
    return this;
  }

  public string Path { get; set; }


  #region Special ShowResult helpers

  public ITag ShowResultJs(string source) { return ShowResult(source, "javascript"); }
  public ITag ShowResultHtml(string source) { return ShowResult(source, "html"); }
  public ITag ShowResultText(string source) { return ShowResult(source, "text"); }
  // Special use case for many picture / image tutorials
  public ITag ShowResultImg(object tag) {
    var cleaned = tag.ToString()
      .Replace(">", ">\n")
      .Replace("' ", "' \n")
      .Replace(",", ",\n");
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

  #region Snippet

  // Standalone call
  public ITag Snippet(string snippet) {
    return ShowFileContents(null, snippet, expand: true);
  }

  private const string ResultTabName = ""; // must match js in img samples
  private const string SourceTabName = "-source-code";
  public ITag SnippetStart(string prefix, params string[] names) {
    return Tag.RawHtml(
      // Tab headers
      AutoSnippetTabs(prefix, names),
      // Tab bodies - must open the first one
      Tag.Div().Class("tab-content p-3 border border-top-0 bg-light mb-5").Id("myTabContent").TagStart,
      // Open the first tab-body item as the snippet is right after this
      "  " + Tag.Div().Class("tab-pane fade show active").Id(prefix + ResultTabName)
        .Attr("role", "tabpanel").Attr("aria-labelledby", prefix + "-tab").TagStart
    );
  }

  public ITag SnippetEnd(string prefix) {
    return SnippetEnd(prefix, true);
  }
  


  private ITag SnippetStartEnd() {
    return Tag.RawHtml("  </div>");
  }

  private ITag SnippetMore(string prefix, string id, object result) {
    return Tag.RawHtml(
      "\n",
      Tag.Div().Class("tab-pane fade").Id(prefix + id)
        .Attr("role", "tabpanel").Attr("aria-labelledby", prefix + id + "-tab").Wrap(
          result
        ),
      "\n"
    );
  }
  private ITag SnippetEnd(string prefix, bool closeStart) {
    return Tag.RawHtml(
      closeStart ? SnippetStartEnd() : Tag.RawHtml(),
      SnippetMore(prefix, SourceTabName, Snippet(prefix)),
      "</div>"
    );
  }

  #region Snippet Inline

  public ITag SnippetInlineStart(string prefix) {
    _inlineId = prefix;
    return Tag.RawHtml(
      Tag.Div().Class("alert alert-info").TagStart,
      Tag.H4("Output")
    );
  }
  private string _inlineId = null;

  public ITag SnippetInlineEnd() {
    var result = Text.Has(_inlineId) ? Snippet(_inlineId) : Tag.Div("Error - can't close inline snippet without ...Start first");
    return Tag.RawHtml(
      "</div>",
      Text.Has(_inlineId) ? Snippet(_inlineId) : Tag.Div("Error - can't close inline snippet without ...Start first")
    );
    _inlineId = null;
  }
  #endregion

  #region Snippet Only
  public ITag SnippetOnlyStart(string prefix) {
    return Snippet(prefix);
  }
  public ITag SnippetOnlyEnd() {
    return null;
  }

  #endregion

  public ITag ResultStart(string prefix, params string[] names) {
    _lastNames = names;
    return SnippetStart(prefix, names);
  }
  private string[] _lastNames;
  private string GetResultName(int index) {
    var l = Log.Call<string>("index:" + index);
    if (_lastNames == null || !_lastNames.Any()) return l("no names", "unknown");
    if (_lastNames.Length < index + 1) return l("index to high", "unknown");
    var name = _lastNames[index];
    Log.Add("name before optimization: '" + name + "'");
    var result = "-" + name.ToLower().Replace(" ", "-");
    return l(result, result);
  }

  public string ResultPrepare() { return ""; }

  public ITag ResultEnd(string prefix, params object[] results) {
    var l = Log.Call<ITag>("prefix: " + prefix + ", results:" + results.Length);
    var nameCount = 0;
    var html = Tag.RawHtml();
    if (results != null && results.Any())
    {
      // Close the tabs / header div section
      html.Add(SnippetStartEnd());
      foreach(var m in results) {
        var name = GetResultName(nameCount);
        Log.Add("tab name:" + name + " (" + nameCount + ")");
        html.Add(SnippetMore(prefix, name, m));
        nameCount++;
      }
    }
    _lastNames = null;
    html.Add(SnippetEnd(prefix, false));
    return l("resulting Html", html);
  }

  #endregion

  #region Snippet Tabs to select between snippet and source

  public ITag AutoSnippetButton(string prefix, string title, string name, bool selected) {
    return Tag.Button(title).Class("nav-link " + (selected ? "active" : "")).Id(prefix + "-tab")
      .Attr("data-bs-toggle", "tab")
      .Attr("data-bs-target", "#" + prefix + name)
      .Type("button")
      .Attr("role", "tab")
      .Attr("aria-controls", prefix + name)
      .Attr("aria-selected", selected.ToString().ToLower());
  }

  private ITag AutoSnippetTabs(string prefix, params string[] names) {
    // First Tab
    var result = Tag.Ul().Class("nav nav-pills p-3 rounded-top border").Attr("role", "tablist").Wrap(
      Tab(prefix, ResultTabName, "Output", true)
    );
    // Optional additional tabs
    if (names != null && names.Any())
      foreach(var n in names) 
        result.Add(Tab(prefix, "-" + n.ToLower().Replace(" ", "-"), n));
    // final Source tab
    result.Add(Tab(prefix, SourceTabName, "Source Code"));
    return result;
  }

  private ITag Tab(string prefix, string id, string label, bool active = false) {
    return Tag.Li().Class("nav-item").Attr("role", "presentation").Wrap(
      AutoSnippetButton(prefix, label, id, active)
    );
  }

  #endregion

  #region Show Source Block

  public ITag ShowCurrentRazor() {
    return ShowFileContents("");
  }
  public ITag Show(string file) {
    return ShowFileContents(file);
  }

  public ITag ShowFileContents(string file,
    string snippet = null, string title = null, string titlePath = null, 
    bool? expand = null, bool? wrap = null)
  {
    var debug = false;
    var path = Path;
    var errPath = Path;
    try
    {
      var specs = GetFileAndProcess(path, file, snippet);
      path = specs.Path;  // update in case of error
      errPath = debug ? specs.FullPath : path;
      title = title ?? "Source Code of " + (Text.Has(specs.File)
        ? titlePath + specs.File
        : "this " + specs.Type); // "this snippet" vs "this file"
      specs.Expand = expand ?? specs.Expand;
      specs.Wrap = wrap ?? specs.Wrap;
      return Tag.RawHtml(
        debug ? Tag.Div(errPath).Class("alert alert-info") : null,
        SourceBlock(specs, title),
        TurnOnSource(specs, specs.Path, specs.Wrap)
      );
    }
    catch
    {
      return Tag.Div("Error showing " + errPath).Class("alert alert-warning");
      // throw;
      return ShowError(path);
    }
    return null;
  }


  public SourceInfo GetFileAndProcess(string path, string file, string snippet = null) {
    var fileInfo = GetFile(path, file);
    var source = KeepOnlySnippet(fileInfo.Contents, snippet);
    source = ProcessHideTrimSnippet(source);
    fileInfo.Processed = SourceTrim(source);
    fileInfo.Size = Size(null, fileInfo.Processed);
    var isSnippet = !string.IsNullOrWhiteSpace(snippet);
    fileInfo.WithIntro = !isSnippet;
    fileInfo.Type = isSnippet ? "snippet" : "file";
    fileInfo.DomAttribute = "source-code-" + CmsContext.Module.Id;
    if (string.IsNullOrEmpty(snippet) && string.IsNullOrEmpty(fileInfo.File)) fileInfo.Expand = false;
    return fileInfo;
  }

  private dynamic ShowError(string path) {
    return Tag.RawHtml(
      Tag.H2("Error showing file source"),
      Tag.Div("Where was a problem showing the file source for " + path).Class("alert alert-warning")
    );
  }



  private dynamic SourceBlock(ShowSourceSpecs specs, string title) {

    return Tag.Div().Class("code-block " + (specs.Expand ? "is-expanded" : "")).Attr(specs.DomAttribute).Wrap(
      specs.WithIntro
        ? Tag.Div().Class("header row justify-content-between").Wrap(
            Tag.Div().Class("col-11").Wrap(
              Tag.H2(title),
              Tag.P("Below you'll see the source code of the " + specs.Type + @". 
                  Note that we're just showing the main part, and hiding some parts of the file which are not relevant for understanding the essentials. 
                  <strong>Click to expand the code</strong>")
            ),
            Tag.Div().Class("col-auto").Wrap(
              // Up / Down arrows as SVG - hidden by default, become visible based on CSS 
              Tag.Custom("<img src='" + App.Path + "/assets/svg/arrow-up.svg' class='fa-chevron-up'>"),
              Tag.Custom("<img src='" + App.Path + "/assets/svg/arrow-down.svg' class='fa-chevron-down'>")
            )
          ) as ITag
        : Tag.Span(),
      SourceBlockCode(specs)
    );
  }


  private ITag ShowResult(string source, string language) {
    source = SourceTrim(source);
    var specs = new ShowSourceSpecs() {
      Processed = source,
      Size = Size(null, source),
      Language = language,
    };
    return Tag.Div().Class("pre-result").Wrap(
      SourceBlockCode(specs),
      TurnOnSource(specs, "", false)
    );
  }

  private ITag SourceBlockCode(ShowSourceSpecs specs) {
    return Tag.Div().Class("source-code").Wrap(
      Tag.Pre(Tags.Encode(specs.Processed)).Id(specs.RandomId).Style("height: " + specs.Size + "px; font-size: 16px")
    );
  }

  public ITag TurnOnSource(ShowSourceSpecs specs, string filePath, bool wrap) {
    var language = "ace/mode/" + (specs.Language ?? (Text.Has(filePath)
      ? FindAce3LanguageName(filePath)
      : "html"));

    var turnOnData = new {
      @await = "window.ace",
      run = "window.razorTutorial.initSourceCode()",
      debug = true,
      data = new {
        test = "now-automated",
        domAttribute = specs.DomAttribute,
        aceOptions = new {
          wrap,
          language,
          sourceCodeId = specs.RandomId
        }
      }
    };
    return Tag.Custom("turnOn").Attr("turn-on", turnOnData);
  }


  #endregion

  #region File Processing

  public SourceInfo GetFile(string filePath, string file) {
    var l = Log.Call<SourceInfo>("filePath:" + filePath + ", file:" + file);
    if (Text.Has(file)) {
      if (file.IndexOf(".") == -1)
        file = "_" + file + ".cshtml";
      var lastSlash = filePath.LastIndexOf("/");
      filePath = filePath.Substring(0, lastSlash) + "/" + file;
    }
    var fullPath = filePath;
    if (filePath.IndexOf(":") == -1 && filePath.IndexOf(@"\\") == -1)
      fullPath = GetFullPath(filePath);
    var contents = System.IO.File.ReadAllText(fullPath);
    return l("test", new SourceInfo { File = file, Path = filePath, FullPath = fullPath, Contents = contents });
  }

  public class ShowSourceSpecs {
    public ShowSourceSpecs() {
      RandomId = "source" + Guid.NewGuid().ToString();
    }
    public string Processed;
    public int Size;
    public string Language;
    public string Type = "file";
    public string DomAttribute;
    public string RandomId;
    public bool WithIntro;
    public bool Expand = true;
    public bool Wrap;
  }

  public class SourceInfo : ShowSourceSpecs {
    public string File;
    public string Path;
    public string FullPath;
    public string Contents;
  }

  private string GetFullPath(string filePath) {
    var l = Log.Call<string>(filePath);
    #if NETCOREAPP
      // This is the Oqtane implementation - cannot use Server.MapPath
      // 2sxclint:disable:v14-no-getservice
      var hostingEnv = GetService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();
      var pathWithTrimmedFirstSlash = filePath.TrimStart(new [] { '/', '\\' });
      var result = System.IO.Path.Combine(hostingEnv.ContentRootPath, pathWithTrimmedFirstSlash);
    #else
      var result = HttpContext.Current.Server.MapPath(filePath);
    #endif
    return l(result, result);
  }

  #endregion

  #region Private Source Code Clean-up Helpers

  public string KeepOnlySnippet(string source, string id) {
    if (string.IsNullOrWhiteSpace(id)) return source;
    // trim unnecessary comments
    var patternSnippet = @"(?:<snippet id=""" + id + @"""[^>]*>)(?<contents>[\s\S]*?)(?:</snippet>)";
    var match = Regex.Match(source, patternSnippet);
    if (match.Length > 0) {
      return match.Groups["contents"].Value;
    }
    // V2 with Snippet Tabs / Inline Tabs
    var patternStartEnd = @"(?:@Sys\.SourceCode\.Snippet(Inline|Only|Intro)?Start\(""" + id + @"""[^\)]*\))(?<contents>[\s\S]*?)(?:@Sys\.SourceCode\.Snippet)";
    match = Regex.Match(source, patternStartEnd);
    if (match.Length > 0) {
      return match.Groups["contents"].Value;
    }
    // V2 with Result Tabs
    patternStartEnd = @"(?:@Sys\.SourceCode\.ResultStart\(""" + id + @"""[^\)]*\))(?<contents>[\s\S]*?)(?:@Sys\.SourceCode\.Result)";
    match = Regex.Match(source, patternStartEnd);
    if (match.Length > 0) {
      return match.Groups["contents"].Value;
    }


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
    source = ProcessHideSilent(source, @"@Sys\.SourceCode\.ResultPrepare\(\)", @"@Sys.SourceCode.ResultEnd\(", true, false);

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

  private string SourceTrim(string source) {
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
  private int Size(object sizeObj, string source) {
    var size = Kit.Convert.ToInt(sizeObj, fallback: -1);
    if (size == -1) {
      var sourceLines = source.Split('\n').Length;
      size = sourceLines * LineHeightPx + BufferHeightPx;
    }

    if (size < LineHeightPx) size = 600;
    return size;
  }

  // Determine the ace9 language of the file
  private string FindAce3LanguageName(string filePath) {
    var extension = filePath.Substring(filePath.LastIndexOf('.') + 1);
    switch (extension)
    {
      case "cs": return "csharp";
      case "js": return "javascript";
      case "json": return "json";
      default: return "razor";
    }
  }

  #endregion
}