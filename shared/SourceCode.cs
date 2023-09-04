using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using ToSic.Sxc.Code;
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
  private const string ResultTabName = "Output"; // must match js in img samples, where it becomes "-output"
  private const string SourceTabName = "Source Code";
  private const string ResultAndSourceTabName = "Output and Source";

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

  #region All Main Entry Points to Show Snippets in Various Ways

  /// <summary>
  /// Show tabs with snippets, resulting in Output / Source Code
  /// </summary>
  /// <param name="tabs"></param>
  /// <returns></returns>
  public TabsWithSnippetsSection TabsWithSnippet(Dictionary<string, string> tabs = null) { return new TabsWithSnippetsSection(this, tabs, combineOutputAndSource: false); }
  public TabsWithSnippetsSection TabsWithSnippet(string tabs) { return TabsWithSnippet(TabStringToDic(tabs)); }

  public TabsWithSnippetsSection TabsOutputAndSource(Dictionary<string, string> tabs = null) { return new TabsWithSnippetsSection(this, tabs, combineOutputAndSource: true); }

  public TabsWithSnippetsSection TabsOutputAndSource(string tabs) { return TabsOutputAndSource(TabStringToDic(tabs)); }

  private Dictionary<string, string> TabStringToDic(string tabs) {
    var tabList = tabs.Split(',').Select(t => t.Trim()).ToArray();
    var tabDic = tabList.ToDictionary(
      t => {
        if (t.StartsWith("Rzr14"))
          return "Rzr14";
        if (t.StartsWith("file:"))
          return Text.AfterLast(t, "/") ?? Text.AfterLast(t, ":");
        return t;
      },
      t => (t.StartsWith("Rzr14") ? ("file:./" + AltCodeFileRzr14() + Text.After(t, "Rzr14")) : t)
    );
    return tabDic;
  }

  public SnippetWithIntroSection Intro() {
    return new SnippetWithIntroSection(this, Tag.RawHtml(
        Tag.H4("Initial Code"),
        Tag.P("The following code runs at the beginning and creates some variables/services used in the following samples."))
    );
  }

  public SnippetWithIntroSection OutputBoxAndSnippet() {
    return new SnippetWithIntroSection(this, Tag.H4("Output"));
  }


  /// <summary>
  /// Lightweight "Just show a Snippet" object
  /// </summary>
  /// <returns></returns>
  public SnippetOnlySection SnippetOnly() { return new SnippetOnlySection(this); }

  /// <summary>
  /// Dummy SnipEnd for all use cases where the end doesn't need output but marks the end.
  /// </summary>
  public ITag SnipEnd() { return null; }

  /// <summary>
  /// QuickRef section - only to be used in the Quick Reference
  /// </summary>
  /// <param name="tabs"></param>
  /// <param name="tutorials">list of tutorials view IDs to add to the last tab</param>
  /// <returns></returns>
  public QuickRefSection QuickRef(Dictionary<string, string> tabs = null, string[] tutorials = null) {
    return new QuickRefSection(this, tabs, tutorials, split: false);
  }

  public QuickRefSection QuickRefSplit(Dictionary<string, string> tabs = null, string[] tutorials = null, int outputW = 50) {
    return new QuickRefSection(this, tabs, tutorials, split: true, firstWidth: outputW);
  }

  public QuickRefSection QuickRefSplit33(Dictionary<string, string> tabs = null, string[] tutorials = null) {
    return new QuickRefSection(this, tabs, tutorials, split: true, firstWidth: 33);
  }

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


  #endregion

  #region Formula


  public FormulaSection Formula(object specs) { return new FormulaSection(this, specs); }

  #endregion

  #region SectionBase

  /// <summary>
  /// Base class for all code sections with snippets etc.
  /// </summary>
  public abstract class SectionBase {
    public SectionBase(SourceCode sourceCode, Dictionary<string, string> tabs) {
      ScParent = sourceCode;
      BsTabs = ScParent.BsTabs;
      Log = ScParent.Log;
      Tabs = tabs ?? new Dictionary<string, string>();
    }
    internal SourceCode ScParent;
    private dynamic BsTabs;
    internal ICodeLog Log;
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

    protected ITag SnippetStartInner(string firstName, string[] names = null, string active = null) {
      names = names ?? new string[] { };
      var firstIsActive = active == null || active == firstName;
      return Tag.RawHtml(
        TabsForSnippet(firstName, names, active),    // Tab headers
        BsTabs.TabContentGroupOpen(),     // Tab bodies - must open the first one
        "  ",                             // Open the first tab-body item as the snippet is right after this
        BsTabs.TabContentOpen(TabPrefix, Name2TabId(firstName), true, firstIsActive)
      );
    }

    // Tabs for Output, (optional more tabs), Source Code
    private ITag TabsForSnippet(string firstName, string[] names, string active = null) {
      var tabNames = new List<string>() { firstName };
      if (names != null && names.Any())
        tabNames.AddRange(names.Select(n => {
          if (n == "R14" || n == "Rzr14") return "Razor14 and older";
          if (n.EndsWith(".csv.txt")) return n.Replace(".csv.txt", ".csv");
          return n;
        }));
      return BsTabs.TabList(TabPrefix, tabNames, active) as ITag;
    }

    protected virtual string SnippetBefore { get { return null; } }
    protected virtual string SnippetAfter { get { return null; } }

    protected ITag SnipEndFinal(bool snippetInResultTab, bool showSnippetTab, bool endWithSnippet, object[] results, string active) {
      var l = Log.Call<ITag>("snippetInResultTab: " + snippetInResultTab 
        + "; ...inTab: " + showSnippetTab 
        + "; endWithSnippet: " + endWithSnippet 
        + "; snippetId: " + SnippetId 
        + "; tab:" + TabPrefix 
        + "; results:" + results.Length);
      var nameCount = 0;
      // Close the tabs / header div section if it hasn't been closed yet
      var html = Tag.RawHtml();
      // 2023-08-30 previously this was a more global variable, but I believe it's not used any more
      // bool _resultEndClosesReveal = false;
      // if (_resultEndClosesReveal) {
      //   html = html.Add("</div>");
      //   _resultEndClosesReveal = false;
      // }
      if (snippetInResultTab)
        html = html.Add("</div>", SnippetBefore, ScParent.ShowSnippet(SnippetId), SnippetAfter);
      html = html.Add(BsTabs.TabContentClose());

      if (showSnippetTab) {
        html = html.Add(BsTabs.TabContent(TabPrefix, Name2TabId(SourceTabName), ScParent.ShowSnippet(SnippetId), isFirst: false, isActive: active == SourceTabName));
        nameCount++;
      }
      // If we have any results, add them here
      foreach (var m in results) {
        var name = Name2TabId(BsTabs.GetTabName(nameCount + 1));
        Log.Add("tab name:" + name + " (" + nameCount + ")");
        html = html.Add(BsTabs.TabContent(TabPrefix, name, FlexibleResult(m), isFirst: false, isActive: active == name));
        nameCount++;
      }
      html = html.Add(endWithSnippet ? SnippetEndInternal() as object : BsTabs.TabContentGroupClose());
      return l(html, "ok");
    }

    protected ITag SnippetEndInternal() {
      return Tag.RawHtml(
        BsTabs.TabContentClose(),     // Will close if still open
        BsTabs.TabContent(TabPrefix, Name2TabId(SourceTabName), ScParent.ShowSnippet(SnippetId), isFirst: false, isActive: false),
        BsTabs.TabContentGroupClose() // Will close if still open
      );
    }

    // Take a result and if it has a special prefix, process that
    private object FlexibleResult(object result) {
      // If it's a string such as "file:abc.cshtml" then resolve that first
      if (result is string) {
        var strResult = result as string;
        if (strResult.StartsWith("file:"))
          return ScParent.ShowFileContents(((string)result).Substring(5), withIntro: false, showTitle: true);
      } 
      return result;
    }

    // WARNING: DUPLICATE CODE BootstrapTabs.cs / SourceCode.cs; keep in sync
    private static string Name2TabId(string name) {
      return "-" + name.ToLower()
        .Replace(" ", "-")
        .Replace(".", "-")
        .Replace("/", "-")
        .Replace("\\", "-");
    }

  }

  #endregion


  #region TabsWithSnippetsSection

  public class TabsWithSnippetsSection: SectionBase
  {
    public TabsWithSnippetsSection(SourceCode sourceCode, Dictionary<string, string> tabs, bool combineOutputAndSource): base(sourceCode, tabs)
    {
      _combineOutputAndSource = combineOutputAndSource;
    }

    private bool _combineOutputAndSource;

    public override ITag SnipStart(string snippetId = null) {
      // Neutralize snippetId, set TabPrefix etc.
      InitSnippetAndTabId(snippetId);
      var firstTabName = _combineOutputAndSource ? ResultAndSourceTabName : ResultTabName;
      var tabs = Tabs.Keys.ToList();
      if (!_combineOutputAndSource) tabs.Add(SourceTabName);
      var start = SnippetStartInner(firstTabName, tabs.ToArray());
      if (_combineOutputAndSource)
        start = Tag.RawHtml(
          start,
          Tag.Div().Class("alert alert-info").TagStart,
          Tag.H4("Output")
        );
      return start;
    }

    public override ITag SnipEnd() { return SnipEndShared(); }

    private ITag SnipEndShared() {
      // Original setup, without any tabs
      if (!Tabs.Any()) 
        return SnippetEndInternal();

      // Extending 2023-08-29 - with tabs
      var tabContents = new List<object>();
      tabContents.AddRange(Tabs.Values);
      ScParent.Log.Add("tabContents: " + tabContents.Count);
      var result = SnipEndFinal(
        snippetInResultTab: _combineOutputAndSource,
        showSnippetTab: false,
        endWithSnippet: !_combineOutputAndSource,
        results: _contentsOverride ?? tabContents.ToArray(),
        active: SourceTabName
      );
      return result;
    }
    private object[] _contentsOverride = null;

    public ITag SnipEnd(params object[] generated) {
      // must check for any, because generated is `params` and so never null
      _contentsOverride = generated.Any() ? generated : null;
      return SnipEndShared();
    }

  }

  #endregion


  #region class SectionBase, SnippetWithIntroSection, SnippetOnlySection, Formula

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

    public override ITag SnipEnd() { return Tag.RawHtml("</div>", ScParent.ShowSnippet(SnippetId)); }
  }


  public class SnippetOnlySection: SectionBase
  {
    public SnippetOnlySection(SourceCode sourceCode): base(sourceCode, null) { }

    public override ITag SnipStart(string snippetId = null) {
      // Neutralize snippetId, set TabPrefix etc.
      InitSnippetAndTabId(snippetId);
      return ScParent.ShowSnippet(SnippetId);
    }
  }


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
        ? SnippetStartInner(ResultTabName, new [] { "Formulas", SourceTabName })
        : SnippetStartInner(ResultTabName, new [] { "Formulas" });
    }

    public override ITag SnipEnd()
    {
      var l = ScParent.Log.Call<ITag>();
      if (Specs != null) {
        bool showSnippet = ScParent.Formulas.ShowSnippet(Specs);
        var result = SnipEndFinal(
          snippetInResultTab: false,
          showSnippetTab: false,
          endWithSnippet: showSnippet,
          results: new object[] { ScParent.Formulas.Show(Specs, false) },
          active: null
      );
        return l(result, "formula show snippet");
      }

      var result3 = SnipEndFinal(false, false, false, results: null, active: null);
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

  public class QuickRefSection: SectionBase
  {
    internal QuickRefSection(
      SourceCode sourceCode,
      Dictionary<string, string> tabs,
      string[] tutorials,
      bool split = false,
      int firstWidth = 50
    ) : base(sourceCode, tabs)
    {
      Tutorials = tutorials ?? new string[] {};
      Split = split;
      FirstWidth = firstWidth;
    }
    private string [] Tutorials;

    private bool Split;
    private int FirstWidth;

    public override ITag SnipStart(string snippetId = null) {
      InitSnippetAndTabId(snippetId);
      var firstTab = Split ? ResultAndSourceTabName : ResultTabName;
      var active = Split ? ResultAndSourceTabName : SourceTabName;
      var list = new List<string>();
      if (!Split) list.Add(SourceTabName);
      list.AddRange(Tabs.Keys.ToArray());

      if (Tutorials != null && Tutorials.Any())
        list.Add("Additional Tutorials");

      var header = SnippetStartInner(firstTab, list.ToArray(), active: active);
      if (!Split) return header;

      return Tag.RawHtml(
        header,
        "\n",
        IndentPreSplit,
        "<!-- Splitter -->",
        "\n",
        IndentPreSplit,
        Tag.Div().Id(TabPrefix + "-splitter").Class("splitter-group").TagStart,
        "\n",
        IndentSplit,
        "<!-- split left -->\n",
        IndentSplit,
        Tag.Div().Id(TabPrefix + "-splitter-left").TagStart,
        "\n",
        IndentSplit,
        Tag.H4("Output"),
        IndentPreSplit
      );
    }

    private const string IndentPreSplit = "      ";

    private const string IndentSplit = "        ";

    public override ITag SnipEnd() {
      var tabContents = new List<object>();
      tabContents.AddRange(Tabs.Values);
      var liLinks = Tutorials.Select(r => "\n    " + ScParent.Sys.TutorialLiLinkLookup(r) + "\n");
      var olLinks = Tag.Ol(liLinks);
      tabContents.Add(olLinks);
      var result = SnipEndFinal(
        snippetInResultTab: Split /* false */, // if split, the result must be inside...
        showSnippetTab: !Split /* true */,
        endWithSnippet: false,
        results: tabContents.ToArray(),
        active: SourceTabName
      );
      // return result;
      if (!Split) return result;

      ScParent.Kit.Page.TurnOn("window.splitter.init()", data: new {
        parts = new [] {
          "#" + TabPrefix + "-splitter-left",
          "#" + TabPrefix + "-splitter-right"
        },
        options = new {
          sizes = new [] { FirstWidth, 100 - FirstWidth },
        }
      });

      return Tag.RawHtml(
        result,
        "\n",
        "<!-- /Splitter -->"
      );
    }

    protected override string SnippetBefore { get {
      return Split
        ? "\n" + IndentSplit + "<!-- /split-left -->" 
          + "\n" + IndentSplit + "<!-- split-right -->"
          + "\n" + IndentSplit
          + Tag.Div().Id(TabPrefix + "-splitter-right").TagStart.ToString()
          + "\n"
        : null;
    } }
    protected override string SnippetAfter { get {
      return Split
        ? IndentSplit 
          + "</div>\n" 
          + IndentSplit
          + "<!-- /split-right -->\n"
          + "\n" + IndentPreSplit 
          + "</div>\n"
          + IndentPreSplit
          + "<!-- /Splitter -->\n"
        : null;
    } }
  }

  #endregion


  public string Invisible() { return null; }



  #region Show Source Block

  public ITag ShowFile(string file, string titlePath = null) {
    return ShowFileContents(file, titlePath: titlePath);
  }

  /// <summary>
  /// just show a snippet
  /// </summary>
  private ITag ShowSnippet(string snippetId) {
    return ShowFileContents(null, snippetId, expand: true);
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

    // New: support for snippetId in path
    if (snippetId == null && file.Contains("#")) {
      snippetId = Text.AfterLast(file, "#");
      file = Text.BeforeLast(file, "#");
    }

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
        "\n<!-- Source Code -->\n",
        SourceBlock(specs, title),
        "\n<!-- /Source Code -->\n"
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
      "\n<!-- Raw Source in Pre -->\n",
      SourceBlockCode(specs),
      "\n<!-- /Raw Source in Pre -->\n"
    );
  }



  private ITag SourceBlockCode(ShowSourceSpecs specs) {
    return Tag.Div().Class("source-code").Wrap(
      "\n",
      Tag.Pre(Tags.Encode(specs.Processed)).Id(specs.RandomId).Style("height: " + specs.Size + "px; font-size: 16px"),
      "\n"
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

  #region (private/internal) File Processing

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

  internal class ShowSourceSpecs {
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

  internal class SourceInfo : ShowSourceSpecs {
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

  #region (private) Source Code Clean-up Helpers

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

  #region Support Rrz14 Debugging

  public bool ShouldShowAltCodeFile()
  {
    return MyPage.Parameters.ContainsKey("Rzr14");
  }

  public ITag ShowAltCodeFile(dynamic html) {
    var altFile = AltCodeFileRzr14();
    return Tag.RawHtml(Tag.H1("Show Rzr"), Tag.Code(altFile), html.Partial(altFile));
  }

  public string AltCodeFileRzr14()
  {
    return Text.AfterLast(Path, "/").Replace(".cshtml", ".Rzr14.cshtml");
  }

  #endregion
}