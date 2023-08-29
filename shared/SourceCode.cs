using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class SourceCode: Custom.Hybrid.CodeTyped
{
  #region Constants

  const int LineHeightPx = 20;
  const int BufferHeightPx = 20; // for footer scrollbar which often appears

  #endregion

  #region Init / Dependencies

  public SourceCode Init(dynamic sys, string path) {
    Sys = sys;
    Path = path;
    SourceProcessor = GetCode("SourceProcessor.cs");
    BsTabs = GetCode("BootstrapTabs.cs");
    return this;
  }

  public dynamic Sys {get;set;}
  public string Path { get; set; }
  private dynamic SourceProcessor { get; set; }
  private dynamic BsTabs {get;set;}

  public dynamic Formulas { get { return _formulas ?? (_formulas = GetCode("SourceCodeFormulas.cs")).Init(this); } }
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

  public TabsWithSnippetsSection TabsWithSnippet(Dictionary<string, string> tabs = null) { return new TabsWithSnippetsSection(this, tabs); }

  public class TabsWithSnippetsSection: SectionBase
  {
    public TabsWithSnippetsSection(SourceCode sourceCode, Dictionary<string, string> tabs): base(sourceCode, tabs) { }

    public override ITag SnipStart(string snippetId = null) {
      // Neutralize snippetId, set TabPrefix etc.
      InitSnippetAndTabId(snippetId);
      return ScParent.SnippetStartInner(TabPrefix, ResultTabName, SourceTabName, Tabs.Keys.ToArray());
    }

    public override ITag SnipEnd() {
      var result = ScParent.SnippetEndInternal(TabPrefix, SnippetId);
      return result;
    }

  }

  public ITag SnippetStart(string snippetId, params string[] names) {
    return SnippetStartInner(snippetId, ResultTabName, SourceTabName, names);
  }

  internal ITag SnippetStartInner(string tabIdPrefix, string firstName, string lastName, string[] names = null, string active = null) {
    names = names ?? new string[] { };
    _snippet = tabIdPrefix;
    var firstIsActive = active == null || active == firstName;
    return Tag.RawHtml(
      TabsForSnippet(tabIdPrefix, firstName, lastName, names, active),    // Tab headers
      BsTabs.TabContentGroupOpen(),     // Tab bodies - must open the first one
      "  ",                             // Open the first tab-body item as the snippet is right after this
      BsTabs.TabContentOpen(tabIdPrefix, Name2TabId(firstName), true, firstIsActive)
    );
  }

  public ITag SnippetEnd() {
    return SnippetEndInternal(_snippet, _snippet);
  }

  private ITag SnippetEndInternal(string snippetTabId, string snippetId) {
    return Tag.RawHtml(
      BsTabs.TabContentClose(),     // Will close if still open
      BsTabs.TabContent(snippetTabId, Name2TabId(SourceTabName), Snippet(snippetId), isFirst: false, isActive: false),
      BsTabs.TabContentGroupClose() // Will close if still open
    );
  }

  // Tabs for Output, (optional more tabs), Source Code
  private ITag TabsForSnippet(string tabIdPrefix, string firstName, string lastName, string[] names, string active = null) {
    var tabNames = new List<string>() { firstName };
    if (names != null && names.Any())
      tabNames.AddRange(names.Select(n => n == "R14" ? "Razor14 and older" : n));
    if (Text.Has(lastName))
      tabNames.Add(lastName);
    return BsTabs.TabList(tabIdPrefix, tabNames, active) as ITag;
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
  public SnippetWithIntroSection Intro() {
    return new SnippetWithIntroSection(this, Tag.RawHtml(
        Tag.H4("Initial Code"),
        Tag.P("The following code runs at the beginning and creates some variables/services used in the following samples."))
    );
  }

  public SnippetWithIntroSection OutputBoxAndSnippet() {
    return new SnippetWithIntroSection(this, Tag.H4("Output"));
  }

  public class SnippetWithIntroSection: SectionBase
  {
    public SnippetWithIntroSection(SourceCode sourceCode, ITag intro): base(sourceCode, null) {
      Intro = intro;
    }
    private ITag Intro;

    public override ITag SnipStart(string snippetId = null) {
      InitSnippetAndTabId(snippetId);
      return Tag.RawHtml(
        Tag.Div().Class("alert alert-info").TagStart,
        Intro
      );
    }

    public override ITag SnipEnd() { return Tag.RawHtml("</div>", ScParent.Snippet(SnippetId)); }
  }

  #endregion

  #region SnippetOnly()

  /// <summary>
  /// Lightweight "Just show a Snippet" object
  /// </summary>
  /// <returns></returns>
  public SnippetOnlySection SnippetOnly() { return new SnippetOnlySection(this); }

  /// <summary>
  /// Dummy SnipEnd for all use cases where the end doesn't need output but marks the end.
  /// </summary>
  public ITag SnipEnd() { return null; }

  public class SnippetOnlySection: SectionBase
  {
    public SnippetOnlySection(SourceCode sourceCode): base(sourceCode, null) { }

    public override ITag SnipStart(string snippetId = null) {
      // Neutralize snippetId, set TabPrefix etc.
      InitSnippetAndTabId(snippetId);
      return ScParent.Snippet(SnippetId);
    }
  }

  #endregion

  #region Section Base - for all new implementations using objects

  /// <summary>
  /// Base class for all code sections with snippets etc.
  /// </summary>
  public abstract class SectionBase {
    public SectionBase(SourceCode sourceCode, Dictionary<string, string> tabs) {
      // SourceCode = sourceCode;
      ScParent = sourceCode;
      Tabs = tabs ?? new Dictionary<string, string>();
    }
    internal SourceCode ScParent;
    // public SourceCode SourceCode;
    protected int SnippetCount;
    protected string SnippetId;
    protected string TabPrefix;
    internal Dictionary<string, string> Tabs;

    /// <summary>
    /// The SnippetId must be provided here, so it can be found in the source code later on
    /// </summary>
    public abstract ITag SnipStart(string snippetId = null);

    protected void InitSnippetAndTabId(string snippetId = null) {
      SnippetCount = ScParent.SourceCodeTabCount++;
      SnippetId = snippetId ?? "" + SnippetCount;
      TabPrefix = "tab-" + ScParent.UniqueKey + "-" + SnippetCount + "-" + (snippetId ?? "auto-id");
    }

    public virtual ITag SnipEnd() {
      return null;
    }
  }

  #endregion

  #region Formulas and FormulaStart() 

  public FormulaSection Formula(object specs) { return new FormulaSection(this, specs); }

  public class FormulaSection: SectionBase
  {
    public FormulaSection(SourceCode sourceCode, object specs): base(sourceCode, null)
    {
      // If specs is a string, look it up in the DB, otherwise use the given object
      Specs = specs is string ? ScParent.Formulas.Specs(specs as string) : specs;
      // TODO: IF we have tabs, add them here from the specs
    }

    public object Specs;

    /// <summary>
    /// Show the entire formula as configured in the Specs
    /// </summary>
    /// <returns></returns>
    public ITag ShowAll() {
      var result = Tag.RawHtml(
        ScParent.Formulas.Header(Specs),
        SnipStart("formula-" + Guid.NewGuid().ToString()),
        ScParent.Formulas.Intro(Specs),
        SnipEnd()
      );
      return result;
    }


    public override ITag SnipStart(string snippetId = null) {
      // Neutralize snippetId, set TabPrefix etc.
      InitSnippetAndTabId(snippetId);
      // Activate toolbar for anonymous so it will always work in demo-mode
      ScParent.Sys.ToolbarHelpers.EnableEditForAll();

      var showSource = ScParent.Formulas.ShowSnippet(Specs);
      return showSource
        ? ScParent.SnippetStartInner(TabPrefix, ResultTabName, SourceTabName, new [] { "Formulas" })
        : ScParent.SnippetStartInner(TabPrefix, ResultTabName, "Formulas", null);
    }

    public override ITag SnipEnd()
    {
      var l = ScParent.Log.Call<ITag>();
      if (Specs != null) {
        bool showSnippet = ScParent.Formulas.ShowSnippet(Specs);
        var result = ScParent.ResultEndInner2(
          showSnippetInResult: false,
          showSnippetTab: false,
          endWithSnippet: showSnippet,
          results: new object[] { ScParent.Formulas.Show(Specs, false) },
          active: null,
          snippetTabId: TabPrefix,
          snippetId: SnippetId);
        return l(result, "formula show snippet");
      }

      var result3 = ScParent.ResultEndInner2(false, false, false, results: null, active: null, snippetTabId: null, snippetId: null);
      return l(result3, "formula without snippet");
    }
  }

  #endregion

  #region Counter / Identifiers

  /// <summary>
  /// Count of source code snippets - used to create unique IDs
  /// </summary>
  public int SourceCodeTabCount = 0;

  public string UniqueKey { get { return _uniqueKey ?? (_uniqueKey = Kit.Key.UniqueKeyWith(this)); }}
  private string _uniqueKey;

  #endregion

  

  #region Reference / CheatSheet

  public QuickRefSection QuickRef(Dictionary<string, string> tabs = null, string[] tutorials = null) {
    return new QuickRefSection(this, tabs ?? new Dictionary<string, string>(), tutorials);
  }

  public class QuickRefSection: SectionBase
  {
    public QuickRefSection(SourceCode sourceCode, Dictionary<string, string> tabs, string[] tutorials): base(sourceCode, tabs)
    {
      Tutorials = tutorials ?? new string[] {};
    }
    private string [] Tutorials;

    public override ITag SnipStart(string snippetId = null) {
      // Neutralize snippetId, set TabPrefix etc.
      InitSnippetAndTabId(snippetId);
      var list = new List<string>() { SourceTabName };
      list.AddRange(Tabs.Keys.ToArray());
      if (Tutorials != null && Tutorials.Any())
        list.Add("Additional Tutorials");
      return ScParent.SnippetStartInner(TabPrefix, ResultTabName, null, list.ToArray(), SourceTabName);
    }

    public override ITag SnipEnd() {
      var liLinks = Tutorials.Select(r => ScParent.Sys.TutorialLiLinkLookup(r).ToString());
      var olLinks = Tag.Ol(liLinks);
      var tabContents = new List<object>();
      tabContents.AddRange(Tabs.Values);
      tabContents.Add(olLinks);
      var result = ScParent.ResultEndInner2(false, true, false,
        results: tabContents.ToArray(),
        active: SourceTabName,
        snippetTabId: TabPrefix,
        snippetId: SnippetId
      );
      return result;
    }
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

  public ITag ResultEnd(params object[] results) { return ResultEndInner(true, results); }

  private ITag ResultEndInner(bool showSnippet, object[] results) {
    var l = Log.Call<ITag>("showSnippet: " + showSnippet + "; prefix: " + _snippet + "; results:" + results.Length);
    return l(ResultEndInner2(showSnippet && _resultEndWillPrepend, false, showSnippet && !_resultEndWillPrepend, results, active: null, snippetTabId: _snippet, snippetId: _snippet), "ok");
  }

  private ITag ResultEndInner2(bool showSnippetInResult, bool showSnippetTab, bool endWithSnippet, object[] results, string active, string snippetTabId, string snippetId) {
    var l = Log.Call<ITag>("showSnippetInResult: " + showSnippetInResult + "; ...inTab: " + showSnippetTab + "; endWithSnippet: " + endWithSnippet + "; snippetId: " + snippetId + "; tab:" + snippetTabId + "; results:" + results.Length);
    var nameCount = 0;
    // Close the tabs / header div section if it hasn't been closed yet
    var html = Tag.RawHtml();
    if (_resultEndClosesReveal) {
      html = html.Add("</div>");
      _resultEndClosesReveal = false;
    }
    if (showSnippetInResult)
      html = html.Add("</div>", Snippet(snippetId));
    html = html.Add(BsTabs.TabContentClose());

    if (showSnippetTab) {
      html = html.Add(BsTabs.TabContent(snippetTabId, Name2TabId(SourceTabName), Snippet(snippetId), isFirst: false, isActive: active == SourceTabName));
      nameCount++;
    }
    // If we have any results, add them here
    foreach (var m in results) {
      var name = Name2TabId(BsTabs.GetTabName(nameCount + 1));
      Log.Add("tab name:" + name + " (" + nameCount + ")");
      html = html.Add(BsTabs.TabContent(snippetTabId, name, FlexibleResult(m), isFirst: false, isActive: active == name));
      nameCount++;
    }
    html = html.Add(endWithSnippet ? SnippetEndInternal(snippetTabId, snippetId) as object : BsTabs.TabContentGroupClose());
    return l(html, "ok");
  }


  #endregion


  #region Show Source Block

  public ITag ShowFile(string file, string titlePath = null) {
    return ShowFileContents(file, titlePath: titlePath);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="file">The file name - if empty string, will use current file name</param>
  /// <param name="snippetId"></param>
  /// <param name="title"></param>
  /// <param name="titlePath"></param>
  /// <param name="expand"></param>
  /// <param name="wrap"></param>
  /// <param name="withIntro"></param>
  /// <param name="showTitle"></param>
  /// <returns></returns>
  private ITag ShowFileContents(string file,
    string snippetId = null, string title = null, string titlePath = null, 
    bool? expand = null, bool? wrap = null, bool? withIntro = null, bool? showTitle = null)
  {
    var debug = false;
    var path = Path;
    var errPath = Path;
    try
    {
      var specs = GetFileAndProcess(path, file, snippetId);
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


  private SourceInfo GetFileAndProcess(string path, string file, string snippetId = null) {
    var fileInfo = GetFile(path, file);
    fileInfo.Processed = SourceProcessor.CleanUpSource(fileInfo.Contents, snippetId);
    fileInfo.Size = Size(null, fileInfo.Processed);
    var isSnippet = !string.IsNullOrWhiteSpace(snippetId);
    fileInfo.ShowIntro = !isSnippet;
    fileInfo.ShowTitle = !isSnippet;
    fileInfo.Type = isSnippet ? "snippet" : "file";
    fileInfo.DomAttribute = "source-code-" + MyContext.Module.Id;
    if (string.IsNullOrEmpty(snippetId) && string.IsNullOrEmpty(fileInfo.File)) fileInfo.Expand = false;
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
              Tag.Custom("<img src='" + App.Folder.Url + "/assets/svg/arrow-up.svg' class='fa-chevron-up' loading='lazy'>"),
              Tag.Custom("<img src='" + App.Folder.Url + "/assets/svg/arrow-down.svg' class='fa-chevron-down' loading='lazy'>")
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

  private SourceInfo GetFile(string filePath, string file) {
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