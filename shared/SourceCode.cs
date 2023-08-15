using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class SourceCode: Custom.Hybrid.Code14
{
  const int LineHeightPx = 20;
  const int BufferHeightPx = 20; // for footer scrollbar which often appears

  #region Init / Dependencies
  public SourceCode Init(dynamic sys, string path) {
    Sys = sys;
    Path = path;
    SourceProcessor = CreateInstance("SourceProcessor.cs");
    BsTabs = CreateInstance("BootstrapTabs.cs");
    return this;
  }

  public dynamic Sys {get;set;}
  public string Path { get; set; }
  private dynamic SourceProcessor { get; set; }
  private dynamic BsTabs {get;set;}

  public dynamic Formulas { get { return _formulas ?? (_formulas = CreateInstance("SourceCodeFormulas.cs")).Init(this); } }
  private dynamic _formulas;

  #endregion


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

  #region Tabbed Files

  // Use ShowManyFiles("Tab Name", "something.cshtml", "Tab Name", "else.cshtml")
  public ITag ShowManyFiles(params string[] titlesAndFiles) {
    var names = titlesAndFiles.Where((n, i) => i % 2 == 0).ToArray();
    var files = titlesAndFiles.Where((n, i) => i % 2 == 1).ToArray();
    if (names.Length != files.Length) throw new ArgumentException("parameters must contain even number of values");
    var prefix = "files" + Name2TabId(names.First()) + "-many-files";
    var contents = BsTabs.TabContentGroup() as Div;
    for(var i = 0; i < names.Length; i++) {
      contents = contents.Add(
        "\n",
        BsTabs.TabContent(prefix, Name2TabId(names[i]),
          ShowFileContents(files[i], withIntro: false, showTitle: true),
          // todo: this isn't quite right yet
          isFirst: i == 0,
          isActive: i == 0
        ),
        "\n"
      );
    }
    var result = Tag.RawHtml(
      BsTabs.TabList(prefix, names) as ITag,
      "\n",
      contents
    );
    return result;
  }

  #endregion

  private const string ResultTabName = "Output"; // must match js in img samples, where it becomes "-output"
  private const string SourceTabName = "Source Code";
  private const string ResultAndSourceTabName = "Output and Source";

  #region Snippet(...) (Standalone)

  // Standalone call - just show a snippet
  public ITag Snippet(string snippet) {
    return ShowFileContents(null, snippet, expand: true);
  }

  #endregion

  #region Remember current snippet ID
  private string _snippet;
  #endregion

  #region SnippetStart() / SnippetEnd() in Tabs

  public ITag SnippetStart(string snippet, params string[] names) {
    return SnippetStartInner(snippet, ResultTabName, SourceTabName, names);
  }

  private ITag SnippetStartInner(string snippetId, string firstName, string lastName, string[] names = null, string active = null) {
    names = names ?? new string[] { };
    _snippet = snippetId;
    var firstIsActive = active == null || active == firstName;
    return Tag.RawHtml(
      TabsForSnippet(snippetId, firstName, lastName, names, active),    // Tab headers
      BsTabs.TabContentGroupOpen(),     // Tab bodies - must open the first one
      "  ",                             // Open the first tab-body item as the snippet is right after this
      BsTabs.TabContentOpen(snippetId, Name2TabId(firstName), true, firstIsActive)
    );
  }

  public ITag SnippetEnd() {
    return Tag.RawHtml(
      BsTabs.TabContentClose(),     // Will close if still open
      BsTabs.TabContent(_snippet, Name2TabId(SourceTabName), Snippet(_snippet), isFirst: false, isActive: false),
      BsTabs.TabContentGroupClose() // Will close if still open
    );
  }

  // Tabs for Output, (optional more tabs), Source Code
  private ITag TabsForSnippet(string prefix, string firstName, string lastName, string[] names, string active = null) {
    var tabNames = new List<string>() { firstName };
    if (names != null && names.Any())
      tabNames.AddRange(names.Select(n => n == "R14" ? "Razor14 and older" : n));
    if (Text.Has(lastName))
      tabNames.Add(lastName);
    return BsTabs.TabList(prefix, tabNames, active) as ITag;
  }

  #endregion

  // Take a result and if it has a special prefix, process that
  private object FlexibleResult(object result) {
    // If it's a string such as "file:abc.cshtml" then resolve that first
    if (result is string) {
      var strResult = result as string;
      if (strResult.StartsWith("file:"))
        return ShowFileContents(((string)result).Substring(5), withIntro: false, showTitle: true);
      if (strResult.StartsWith("snippet:")) {
        var key = ((string)result).Substring(8);
        if (key.StartsWith("*")) key = _snippet + key.Substring(1);
        return Snippet(key);
      }
    } 
    return result;
  }

  private string Name2TabId(string name) { return "-" + name.ToLower().Replace(" ", "-").Replace(".", "-"); }

  #region Snippet Inline and Intro

  public ITag SnippetInlineStart(string prefix) {
    return SnippetInlineInitStart(prefix, Tag.H4("Output"));
  }

  public ITag SnippetInitStart(string prefix) {
    return SnippetInlineInitStart(prefix, Tag.RawHtml(
      Tag.H4("Initial Code"),
      Tag.P("The following code runs at the beginning and creates some variables/services used in the following samples.")));
  }

  private ITag SnippetInlineInitStart(string prefix, ITag body) {
    _inlineId = prefix;
    return Tag.RawHtml(
      Tag.Div().Class("alert alert-info").TagStart,
      body
    );
  }
  private string _inlineId = null;

  public ITag SnippetInlineEnd() { return SnippetInlineInitEnd(); }
  public ITag SnippetInitEnd() { return SnippetInlineInitEnd(); }
  public ITag SnippetInlineInitEnd() {
    var result = Text.Has(_inlineId) ? Snippet(_inlineId) : Tag.Div("Error - can't close inline/init snippet without ...Start first");
    _inlineId = null;
    return Tag.RawHtml(
      "</div>",
      result
    );
  }

  #endregion

  #region SnippetOnly()

  public ITag SnippetOnlyStart(string prefix) {
    return Snippet(prefix);
  }
  public ITag SnippetOnlyEnd() {
    return null;
  }

  #endregion

  #region Formulas and FormulaStart() 


  private object _formulaSpecs;

  public ITag FormulaShow(object specs) {
    // If we got a name, look it up in the examples
    if (specs is string) specs = Formulas.Specs(specs as string);

    var result = Tag.RawHtml(
      Formulas.Header(specs),
      FormulaStart("formula-" + Guid.NewGuid().ToString(), specs),
      Formulas.Intro(specs),
      FormulaEnd()
    );
    return result;
  }

  public ITag FormulaStart(string snippet, object specs = null) {
    // Remember for future close
    _formulaSpecs = specs;

    // Activate toolbar for anonymous so it will always work in demo-mode
    Sys.ToolbarHelpers.EnableEditForAll();

    var showSource = Formulas.ShowSnippet(specs);
    return showSource
      ? SnippetStartInner(snippet, ResultTabName, SourceTabName, new [] { "Formulas" })
      : SnippetStartInner(snippet, ResultTabName, "Formulas", null);
  }
  public ITag FormulaEnd(params object[] results) {
    var result = _formulaSpecs != null
      ? ResultEndInner(Formulas.ShowSnippet(_formulaSpecs), Formulas.Show(_formulaSpecs, false))
      : ResultEndInner(false, results);
    return result;
  }

  #endregion

  #region Reference / CheatSheet

  // Must begin with the term "Result" to be captured later on when looking for the snippet
  public ITag ResultRefStart(string snippetId, params string[] names) {
    var list = new List<string>() { SourceTabName };
    if (names != null && names.Any()) list.AddRange(names);
    list.Add("Additional Tutorials");
    return SnippetStartInner(snippetId, ResultTabName, null, list.ToArray(), SourceTabName);
  }

  public ITag ResultRefEnd(string[] linkRefs, params string[] files) {
    var links = linkRefs == null || !linkRefs.Any()
      ? null
      : Tag.Ol(linkRefs.Select(r => Sys.TutorialLiLinkLookup(r).ToString()));
    var tabContents = new List<object>();
    // tabContents.Add(links);
    if (files != null && files.Any())
      tabContents.AddRange(files);
    tabContents.Add(links);
    var result = ResultEndInner(false, true, false, results: tabContents.ToArray(), active: SourceTabName);
    return result;
  }

  #endregion

  #region ResultStart() | ResultAndSnippetStart() / ResultPrepare() / ResultEnd()

  private bool _resultEndWillPrepend = false;
  private bool _resultEndClosesReveal = false;

  public ITag ResultStart(string snippetId, params string[] names) {
    _resultEndWillPrepend = false;
    return SnippetStartInner(snippetId, ResultTabName, SourceTabName, names);
  }

  public ITag ResultAndSnippetStart(string snippetId, params string[] names) {
    _resultEndWillPrepend = true;
    return Tag.RawHtml(
      SnippetStartInner(snippetId, ResultAndSourceTabName, null, names),
      Tag.Div().Class("alert alert-info").TagStart,
      Tag.H4("Output")
    );
  }


  public string Invisible() { return null; }
  // public string ResultPrepare() { return null; }

  public ITag ResultEnd(params object[] results) { return ResultEndInner(true, results); }

  private ITag ResultEndInner(bool showSnippet, params object[] results) {
    var l = Log.Call<ITag>("showSnippet: " + showSnippet + "; prefix: " + _snippet + "; results:" + results.Length);
    return l(ResultEndInner(showSnippet && _resultEndWillPrepend, false, showSnippet && !_resultEndWillPrepend, results, active: null), "ok");
  }

  private ITag ResultEndInner(bool showSnippetInResult, bool showSnippetTab, bool endWithSnippet, object[] results, string active) {
    var l = Log.Call<ITag>("showSnippetInResult: " + showSnippetInResult + "; ...inTab: " + showSnippetTab + "; endWithSnippet: " + endWithSnippet + "; prefix: " + _snippet + "; results:" + results.Length);
    var nameCount = 0;
    // Close the tabs / header div section if it hasn't been closed yet
    var html = Tag.RawHtml();
    if (_resultEndClosesReveal) {
      html = html.Add("</div>");
      _resultEndClosesReveal = false;
    }
    if (showSnippetInResult)
      html = html.Add("</div>", Snippet(_snippet));
    html = html.Add(BsTabs.TabContentClose());

    if (showSnippetTab) {
      html = html.Add(BsTabs.TabContent(_snippet, Name2TabId(SourceTabName), Snippet(_snippet), isFirst: false, isActive: active == SourceTabName));
      nameCount++;
    }
    // If we have any results, add them here
    foreach (var m in results) {
      var name = Name2TabId(BsTabs.GetTabName(nameCount + 1));
      Log.Add("tab name:" + name + " (" + nameCount + ")");
      html = html.Add(BsTabs.TabContent(_snippet, name, FlexibleResult(m), isFirst: false, isActive: active == name));
      nameCount++;
    }
    html = html.Add(endWithSnippet ? SnippetEnd() as object : BsTabs.TabContentGroupClose());
    return l(html, "ok");
  }


  #endregion


  #region Show Source Block

  public ITag ShowCurrentRazor() {
    return ShowFileContents("");
  }
  public ITag Show(string file) {
    return ShowFileContents(file);
  }

  public ITag ShowFile(string file, string titlePath = null) {
    return ShowFileContents(file, titlePath: titlePath);
  }

  public ITag ShowFileContents(string file,
    string snippet = null, string title = null, string titlePath = null, 
    bool? expand = null, bool? wrap = null, bool? withIntro = null, bool? showTitle = null)
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
      specs.ShowIntro = withIntro ?? specs.ShowIntro;
      specs.ShowTitle = showTitle ?? specs.ShowTitle;

      TurnOnSource(specs, specs.Path, specs.Wrap);

      return Tag.RawHtml(
        debug ? Tag.Div(errPath).Class("alert alert-info") : null,
        SourceBlock(specs, title)
      );
    }
    catch
    {
      throw;
      return Tag.Div("Error showing " + errPath).Class("alert alert-warning");
      return ShowError(path);
    }
    return null;
  }


  public SourceInfo GetFileAndProcess(string path, string file, string snippet = null) {
    var fileInfo = GetFile(path, file);
    fileInfo.Processed = SourceProcessor.CleanUpSource(fileInfo.Contents, snippet);
    fileInfo.Size = Size(null, fileInfo.Processed);
    var isSnippet = !string.IsNullOrWhiteSpace(snippet);
    fileInfo.ShowIntro = !isSnippet;
    fileInfo.ShowTitle = !isSnippet;
    fileInfo.Type = isSnippet ? "snippet" : "file";
    fileInfo.DomAttribute = "source-code-" + CmsContext.Module.Id;
    if (string.IsNullOrEmpty(snippet) && string.IsNullOrEmpty(fileInfo.File)) fileInfo.Expand = false;
    return fileInfo;
  }

  private IHtmlTag ShowError(string path) {
    return Tag.RawHtml(
      Tag.H2("Error showing file source"),
      Tag.Div("Where was a problem showing the file source for " + path).Class("alert alert-warning")
    );
  }



  private Div SourceBlock(ShowSourceSpecs specs, string title) {
    return Tag.Div().Class("code-block " + (specs.Expand ? "is-expanded" : "")).Attr(specs.DomAttribute).Wrap(
      specs.ShowIntro
        ? Tag.Div().Class("header row justify-content-between").Wrap(
            Tag.Div().Class("col-11").Wrap(
              Tag.H3(title),
              Tag.P("Below you'll see the source code of the " + specs.Type + @". 
                    Note that we're just showing the main part, and hiding some parts of the file which are not relevant for understanding the essentials. 
                    <strong>Click to expand the code</strong>")
            ),
            Tag.Div().Class("col-auto").Wrap(
              // Up / Down arrows as SVG - hidden by default, become visible based on CSS 
              Tag.Custom("<img src='" + App.Path + "/assets/svg/arrow-up.svg' class='fa-chevron-up' loading='lazy'>"),
              Tag.Custom("<img src='" + App.Path + "/assets/svg/arrow-down.svg' class='fa-chevron-down' loading='lazy'>")
            )
          ) as ITag
        : specs.ShowTitle
          ? Tag.H3(title) as ITag
          : Tag.Span(),
      SourceBlockCode(specs)
    );
  }


  private ITag ShowResult(string source, string language) {
    source = SourceProcessor.SourceTrim(source) as string;
    var specs = new ShowSourceSpecs() {
      Processed = source,
      Size = Size(null, source),
      Language = language,
    };
    TurnOnSource(specs, "", false);
    return Tag.Div().Class("pre-result").Wrap(
      SourceBlockCode(specs)
    );
  }

  private ITag SourceBlockCode(ShowSourceSpecs specs) {
    return Tag.Div().Class("source-code").Wrap(
      Tag.Pre(Tags.Encode(specs.Processed)).Id(specs.RandomId).Style("height: " + specs.Size + "px; font-size: 16px")
    );
  }

  private void TurnOnSource(ShowSourceSpecs specs, string filePath, bool wrap) {
    var language = "ace/mode/" + (specs.Language ?? (Text.Has(filePath)
      ? FindAce3LanguageName(filePath)
      : "html"));

    Kit.Page.TurnOn("window.razorTutorial.initSourceCode()",
      require: "window.ace",
      data: new {
        test = "now-automated",
        domAttribute = specs.DomAttribute,
        aceOptions = new {
          wrap,
          language,
          sourceCodeId = specs.RandomId
        }
      }
    );
  }


  #endregion

  #region File Processing

  public SourceInfo GetFile(string filePath, string file) {
    var l = Log.Call<SourceInfo>("filePath:" + filePath + ", file:" + file);
    if (Text.Has(file)) {
      if (file.IndexOf(".") == -1)
        file = file + ".cshtml";
      var lastSlash = filePath.LastIndexOf("/");
      filePath = filePath.Substring(0, lastSlash) + "/" + file;
    }
    var fullPath = filePath;
    if (filePath.IndexOf(":") == -1 && filePath.IndexOf(@"\\") == -1)
      fullPath = GetFullPath(filePath);
    var contents = System.IO.File.ReadAllText(fullPath);
    return l(new SourceInfo { File = file, Path = filePath, FullPath = fullPath, Contents = contents }, Path);
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
    public bool ShowIntro;
    public bool ShowTitle;
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
      var result = System.Web.HttpContext.Current.Server.MapPath(filePath);
    #endif
    return l(result, result);
  }

  #endregion

  #region Private Source Code Clean-up Helpers

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