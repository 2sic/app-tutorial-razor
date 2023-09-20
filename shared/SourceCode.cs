using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
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
  private const string TutorialsTabName = "Additional Tutorials";
  private const string NotesFieldName = "Notes";
  private const string NotesTabName = "Notes";

  private const string InDepthField = "InDepthExplanation";
  private const string InDepthTabName = "In-Depth Explanation";
  private const string ResultTitle = "Result";
  private const string ViewConfigCode = "ViewConfig";
  private const string ViewConfigTabName = "View Configuration";
  private const string FormulaField = "Formula";
  private const string FormulasTabName = "Formulas";

  private const string IgnoreSourceFile = "ignore";

  #endregion

  #region Init / Dependencies

  public SourceCode Init(dynamic sys, string path) {
    Sys = sys;
    Path = path;
    SourceProcessor = GetCode("SourceProcessor.cs");
    BsTabs = GetCode("BootstrapTabs.cs");
    return this;
  }
  public TagCount TagCount = new TagCount("SourceCode", true);
  public dynamic Sys {get;set;}
  public string Path { get; set; }
  private dynamic SourceProcessor { get; set; }
  private dynamic BsTabs {get;set;}

  public dynamic Formulas { get { return _formulas ?? (_formulas = GetCode("SourceCodeFormulas.cs")).Init(this); } }
  private dynamic _formulas;

  #endregion

  #region All Available Entry Points to Show Snippets in Various Ways

  /// <summary>
  /// QuickRef section - only to be used in the Quick Reference
  /// for manual adding complex cases - ATM not in use 2023-09-12
  /// </summary>
  /// <param name="item">ATM object, because when coming through a Razor14 the type is not known</param>
  /// <param name="tabs"></param>
  /// <returns></returns>
  public QuickRefSection QuickRef(object item, string tabs = null, Dictionary<string, string> tabDic = null)
  {
    var l = Log.Call<QuickRefSection>("tabs: '" + tabs + "'");
    tabDic = tabDic ?? TabStringToDic(tabs);
    var result = new QuickRefSection(this, tabDic, item: item as ITypedItem);
    return l(result, "ok - count: " + tabDic.Count());
  }

  /// <summary>
  /// Create a Snip/Section object from an Item (ITypedItem).
  /// Only used in Accordion
  /// </summary>
  /// <param name="item">The configuration item</param>
  /// <param name="file">The file from which it will be relative to</param>
  /// <returns></returns>
  public QuickRefSection SnipFromItem(ITypedItem item, string file = null)
  {
    var l = Log.Call<QuickRefSection>("file: " + file);
    // If we have a file, we should try to look up the tabs
    var tabCsv = TryToGetTabsFromSource(file);
    Log.Add("tabs: '" + tabCsv + "'");
    var tabs = TabStringToDic(tabCsv);
    var result = new QuickRefSection(this, tabs, item: item, file: file);
    return l(result, "ok - count: " + tabs.Count());
  }


  private Dictionary<string, string> TabStringToDic(string tabs) {
    var tabList = (tabs ?? "").Split(',').Select(t => t.Trim()).ToArray();
    var tabDic = tabList
      .Where(t => t.Has())
      .Select(t => {
        // Pre-Split if possible
        var pair = t.Split('|');
        var hasLabel = pair.Length > 1;
        var pVal = hasLabel ? pair[1] : pair[0];
        var pLabel = pair[0];

        // Figure out the parts
        var label = hasLabel 
          ? pLabel
          : ((t.StartsWith("file:")) ? Text.AfterLast(t, "/") ?? Text.AfterLast(t, ":") : t);
        var value = pVal;
        return new {
          label,
          value,
          t
        };
      })
      .ToDictionary(t => t.label, t => t.value);
    return tabDic;
  }

  private string TryToGetTabsFromSource(string file) {
    if (!file.Has() || file == IgnoreSourceFile) return null;
    var srcPath = file.Replace("\\", "/").BeforeLast("/");
    var src = GetFileAndProcess(file).Contents;
    if (src.Contains("Tut.Tabs=")) {
      var tabsLine = Text.After(src, "Tut.Tabs=");
      var tabsBeforeEol = Text.Before(tabsLine, "\n");
      var tabsString = Text.Before(tabsBeforeEol, "*/") ?? tabsBeforeEol;
      if (!tabsString.Has()) return null;
      var tabs = tabsString.Split(',')
        .Select(t => {
          var entry = t.Trim();
          if (!entry.StartsWith("file:")) return t;
          var fileName = Text.After(entry, "file:");
          return "file:" + srcPath + "/" + fileName;
        })
        .ToArray();
      return string.Join(",", tabs);
    }
    return null;
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


  #region SectionBase

  /// <summary>
  /// Base class for all code sections with snippets etc.
  /// </summary>
  public abstract class SectionBase {
    public SectionBase(SourceCode sourceCode, ITypedItem item, Dictionary<string, string> tabs, string sourceFile = null) {
      ScParent = sourceCode;
      BsTabs = ScParent.BsTabs;
      Log = ScParent.Log;
      Item = item;
      TabHandler = new TabHandlerBase(sourceCode, item, tabs);
      SourceFile = sourceFile;
      ViewConfig = ScParent.GetCode("./ViewConfigurationSimulation.cs").Setup(this.TabHandler);
    }
    internal SourceCode ScParent;
    private dynamic BsTabs;
    internal ICodeLog Log;
    protected int SnippetCount;
    internal string SnippetId {get; private set;}
    public string TabPrefix {get; protected set;}
    // protected Wrap WrapAll;
    protected Wrap SourceWrap;
    internal TabHandlerBase TabHandler;
    internal ITypedItem Item;
    internal string SourceFile;
    public dynamic ViewConfig;

    /// <summary>
    /// The SnippetId must be provided here, so it can be found in the source code later on
    /// </summary>
    public abstract ITag SnipStart(string snippetId = null);

    /// <summary>
    /// Helper which is usually called by implementations of SnipStart
    /// </summary>
    protected void InitSnippetAndTabId(string snippetId = null) {
      SnippetCount = ScParent.SourceCodeTabCount++;
      SnippetId = snippetId ?? "" + SnippetCount;
      TabPrefix = "tab-" + ScParent.UniqueKey + "-" + SnippetCount + "-" + (snippetId ?? "auto-id");
    }

    /// <summary>
    /// End of Snippet, so the source-analyzer can find it.
    /// Usually overridden by the implementation.
    /// </summary>
    public virtual ITag SnipEnd() { return null; }

    protected ITag TabsBeforeContent() {
      var tabNames = TabHandler.TabNames;
      var active = TabHandler.ActiveTabName;
      var l = Log.Call<ITag>("tabs (" + tabNames.Count() + "): " + TabHandler.TabNamesDebug + "; active: " + active);

      var outputOpen = SourceWrap != null ? SourceWrap.OutputOpen() : null;

      if (tabNames.Count() <= 1) return l(outputOpen, "no tabs");

      var firstName = tabNames.FirstOrDefault();
      var firstIsActive = active == null || active == firstName;
      var result = Tag.RawHtml(
        // Tab headers
        BsTabs.TabList(TabPrefix, tabNames, active),
        // Tab bodies - must open the first one
        BsTabs.TabContentGroupOpen(),
        // Open the first tab-body item as the snippet is right after this
        BsTabs.TabContentOpen(TabPrefix, Name2TabId(firstName), firstIsActive),
        // If we have a source-wrap, add it here
        outputOpen
      );
      return l(result, "ok");
    }

    protected ITag SnipEndFinal() {
      var results = TabHandler.TabContents;
      var names = TabHandler.TabNames;
      // Logging
      var active = TabHandler.ActiveTabName;
      var l = Log.Call<ITag>("snippetId: " + SnippetId 
        + "; tabPfx:" + TabPrefix 
        + "; TabNames: " + TabHandler.TabNamesDebug
        + "; results:" + results.Count()
        + "; results:" + TabHandler.TabContentsDebug);
      var nameCount = 0;

      // Close the tabs / header div section if it hasn't been closed yet
      var html = Tag.RawHtml();

      // If we have any results, add them here; Very often there are none left
      foreach (var m in results) {
        string name;
        // Find Name and Log Stuff

        name = names.ElementAt(nameCount);
        nameCount++;
        var msg = "tab name: " + name + " (" + nameCount + ")";
        Log.Add(msg);
        html = html.Add("<!-- " + msg + "-->");

        // Special: if it's the ResultTab (usually the first) - close
        if (name == ResultTabName) {
          Log.Add("Contents of: " + ResultTabName);
          if (SourceWrap != null) html = html.Add(SourceWrap.OutputClose());
          html = html.Add(BsTabs.TabContentClose());
          continue;
        }

        // Special case: Source-and-Result Tab
        if (name == ResultAndSourceTabName) {
          Log.Add("snippetInResultTab - SourceWrap: " + SourceWrap);
          html = html.Add(
            SourceWrap != null ? SourceWrap.OutputClose() : null,
            SourceWrap != null ? SourceWrap.SourceOpen() : null,
            ScParent.ShowSnippet(SnippetId, item: Item, file: SourceFile),
            SourceWrap != null ? SourceWrap.SourceClose() : null
          );
          // Reliably close the "Content" section IF it had been opened
          html = html.Add(BsTabs.TabContentClose());
          continue;
        }

        var nameId = Name2TabId(name);

        // Special case: Source Tab
        if (m == SourceTabName) {
          Log.Add("Contents of: " + SourceTabName);
          html = html.Add(BsTabs.TabContent(TabPrefix, nameId,
            ScParent.ShowSnippet(SnippetId, item: Item, file: SourceFile),
            isActive: active == SourceTabName)
          );
          continue;
        }

        // Other: Normal predefined content
          Log.Add("Contents of: " + name + "; nameId: " + nameId);
        var body = FlexibleResult(m, Item);
        var isActive = active == nameId;
        html = html.Add(BsTabs.TabContent(TabPrefix, nameId, body, isActive: isActive));
      }

      html = html.Add(BsTabs.TabContentGroupClose());
      return l(html, "ok");
    }

    // Take a result and if it has a special prefix, process that
    private object FlexibleResult(object result, ITypedItem item = null)
    {
      // If it's not a string, then it must be something prepared, typically IHtmlTags; return that
      var strResult = result as string;
      if (strResult == null) return result; 

      // If it's a string such as "file:abc.cshtml" then resolve that first
      if (strResult.StartsWith("file:"))
        return ScParent.ShowFileContents(strResult.Substring(5), withIntro: false, showTitle: 
        true);

      // Handle case "html-img:..."
      if (strResult.StartsWith("html-img:"))
        return ScParent.ShowResultImg(strResult.Substring(9));
      // Optionally add tutorial links if defined in the item
      if (item == null) return result;

      // Handle case Tutorials
      if (strResult == TutorialsTabName) {
        var liLinks = Item.Children("Tutorials").Select(tMd => "\n    " + ScParent.Sys.TutorialLiFromViewMd(tMd) + "\n");
        var olLinks = Tag.Ol(liLinks);
        return olLinks;
      }

      // Handle case Notes
      if (strResult == NotesTabName) {
        if (item.IsEmpty(NotesFieldName)) return NotesTabName + " not found";

        var notesHtml = item.Children(NotesFieldName).Select(tMd => Tag.RawHtml(
          "\n    ",
          Tag.Div().Class("alert alert-" + tMd.String("NoteType"))
            .Attr(ScParent.Kit.Toolbar.Empty().Edit(tMd))
            .Wrap(
              Tag.H4(tMd.String("Title")),
              tMd.Html("Note"),
              ScParent.Sys.Fancybox.Gallery(tMd, "Images")
            ),
          "\n"));
        return notesHtml;
      }

      // handle case In-Depth Explanations
      if (strResult == InDepthTabName) {
        if (item.IsEmpty(InDepthField)) return InDepthTabName + " not found";
        return Tag.RawHtml(
          "\n",
          item.String(InDepthField),
          ScParent.Sys.Fancybox.Gallery(item, "InDepthImages"),
          "\n"
        );
      }

      // Handle Case ViewConfig
      if (strResult == ViewConfigCode) {
        return Tag.Div().Wrap(
          Tag.H4(ViewConfigTabName),
          Tag.P("This is how this view would be configured for this sample."),
          ViewConfig.TabContents()
        );
      }

      // handle case Formulas
      if (strResult == FormulasTabName) {
        var formulaSpecs = item.Child(FormulaField);
        return ScParent.Formulas.Show(formulaSpecs, false);
      }

      // Other cases - just return original - could be the label or a prepared string
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

  #region Reference / CheatSheet

  public class QuickRefSection: SectionBase
  {
    internal QuickRefSection(
      SourceCode sourceCode,
      Dictionary<string, string> tabs,
      ITypedItem item = null,
      string file = null
    ) : base(sourceCode, item, tabs, sourceFile: file)
    {
      if (item == null) throw new Exception("Item should never be null");
      // throw new Exception("test 2dm " + item.Type.Name);
      SourceWrap = sourceCode.GetSourceWrap(this, item);
      TabHandler = new TabHandlerBase(sourceCode, item, tabs, sourceWrap: SourceWrap);
    }

    public override ITag SnipStart(string snippetId = null) {
      InitSnippetAndTabId(snippetId);
      return TabsBeforeContent();
    }

    public override ITag SnipEnd() { return SnipEndFinal(); }

    /// <summary>
    /// Public method to replace the tab contents from outside
    /// </summary>
    /// <param name="tabs"></param>
    public void SetTabContents(params object[] tabs) {
      if (tabs != null & tabs.Any()) {
        Log.Add("Replace tab contents with (new): " + tabs.Count());
        TabHandler.ReplaceTabContents(tabs.ToList());
      }
    }
  }



  #endregion


  public class TabHandlerBase
  {
    public TabHandlerBase(SourceCode scParent,
      ITypedItem item,
      Dictionary<string, string> tabs,
      bool addOutput = false,
      bool outputWithSource = false,
      bool sourceAtEnd = false,
      string activeTabName = null,
      Wrap sourceWrap = null
    ) {
      ScParent = scParent;
      Log = ScParent.Log;
      Item = item;
      Tabs = tabs ?? new Dictionary<string, string>();
      SourceWrap = sourceWrap;
      _addOutput = addOutput;
      _outputWithSource = outputWithSource;
      _sourceAtEnd = sourceAtEnd;
      ActiveTabName = (sourceWrap != null)
        ? sourceWrap.TabSelected
        : activeTabName ?? ResultTabName;
    }
    public readonly Dictionary<string, string> Tabs;
    protected readonly SourceCode ScParent; // also used in Formulas
    private ITypedItem Item { get; set; }
    private bool _addOutput;
    private bool _outputWithSource;
    private bool _sourceAtEnd;
    public string ActiveTabName;
    private readonly Wrap SourceWrap;
    private readonly ICodeLog Log;

    #region TabNames

    public List<string> TabNames { get { return _tabNames ?? (_tabNames = GetTabNames()); } }
    private List<string> _tabNames;
    protected virtual List<string> GetTabNames() {
      // Logging
      var l = Log.Call<List<string>>();

      // Build Names
      var names = GetTabNamesAndContentBase(useKeys: true)
        .Select(s => s as string)
        .Where(s => s != null)
        .ToList();
      
      var final = OptimizeTabNames(names);
      return l(final, final.Count.ToString()); 
    }

    protected List<string> OptimizeTabNames(List<string> original) {
      return original.Select(n => {
        if (n == ViewConfigCode) return ViewConfigTabName;
        if (n.EndsWith(".csv.txt")) return n.Replace(".csv.txt", ".csv");
        if (n == InDepthField) return InDepthTabName;
        return n;
      }).ToList();
    }

    private List<object> GetTabNamesAndContentBase(bool useKeys) {
      // Logging
      var l = Log.Call<List<object>>("addOutput:" + _addOutput + 
        "; outputWith: " + _outputWithSource + 
        "; sourceAtEnd: " + _sourceAtEnd
      );
      // Build list
      var list = new List<object>();

      // If we have source-wrap, that determines what tabs to add
      if (this.SourceWrap != null) {
        Log.Add("SourceWrap: " + SourceWrap);
        list.AddRange(SourceWrap.Tabs);
      }
      // Old section, remove as soon as all specs come from the item
        else if (_addOutput) {
          Log.Add("addOutput - old");
          list.Add(_outputWithSource ? ResultAndSourceTabName : ResultTabName);
          if (!_outputWithSource && !_sourceAtEnd) list.Add(SourceTabName);
        }

      if (Tabs != null && Tabs.Any()) {
        Log.Add("Tab Count: " + Tabs.Keys.Count());
        if (useKeys)
          list.AddRange(Tabs.Keys);
        else {
          if (_replaceTabContents != null) {
            list.AddRange(_replaceTabContents);
            // If the tabs have more entries - eg "ViewConfiguration" - then add that at the end as well
            if (Tabs.Values.Count() > _replaceTabContents.Count())
              list.AddRange(Tabs.Values.Skip(_replaceTabContents.Count()));
          }
          else
            list.AddRange(Tabs.Values);
        }
      }

      if (_sourceAtEnd && !_outputWithSource) {
        Log.Add("Add SourceTab:" + SourceTabName);
        list.Add(SourceTabName);
      }

      if (Item != null) {
        if (Item.IsNotEmpty(InDepthField)) {
          Log.Add(InDepthField);
          list.Add(InDepthTabName); // useKeys ? InDepthField : Item.String(InDepthField));
        }
        if (Item.IsNotEmpty(NotesFieldName)) {
          Log.Add(NotesFieldName);
          list.Add(NotesTabName);
        }
        if (Item.IsNotEmpty("Tutorials")) {
          Log.Add("Tutorials");
          list.Add(TutorialsTabName);
        }
      }

      return l(list, list.Count.ToString());
    }


    #endregion

    #region TabContents

    /// <summary>
    /// The TabContents is either automatic, or set by the caller
    /// </summary>
    public List<object> TabContents {
      get { return _tabContents ?? (_tabContents = GetTabContents()) ; }
    }
    private List<object> _tabContents;
    public void ReplaceTabContents(List<object> replacement) {
      var l = Log.Call("replacement: " + (replacement == null ? "null" : "" + replacement.Count()));
      _replaceTabContents = replacement;
      _tabContents = null;  // Reset so it will be regenerated
      l("ok");
    }
    private List<object> _replaceTabContents;
    protected virtual List<object> GetTabContents()
    {
      var l = Log.Call<List<object>>();
      var list = GetTabNamesAndContentBase(useKeys: false);
      return l(list, "tabContents: " + list.Count);
    }

    #endregion

    #region Debug

    public string TabNamesDebug { get { return string.Join(", ", TabNames); }  }
    public string TabContentsDebug { get {
      return string.Join(", ", TabContents.Select(tc => Text.Ellipsis(tc.ToString() ?? "", 20)));
    }  }
    #endregion

  }


  #region class SectionBase, SnippetOnlySection, Formula

  internal class WrapFormula: Wrap
  {
    public WrapFormula(SectionBase sb, ITypedItem specs = null) : base(sb, "WrapFormula") {
      FormulaSpecs = specs ?? Section.Item.Child("Formula");
      if (FormulaSpecs == null) throw new Exception("Formula section needs a Formula item");
      this.Tabs = new List<string> { ResultTabName, FormulasTabName };
      this.TabSelected = ResultTabName;
    }
    public ITypedItem FormulaSpecs;

    public override ITag OutputOpen() {
      // Activate toolbar for anonymous so it will always work in demo-mode
      Section.ScParent.Sys.ToolbarHelpers.EnableEditForAll();
      return Tag.RawHtml(
        Section.ScParent.Formulas.Intro(FormulaSpecs)
      );
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
  


  #region SourceWrappers

  private Wrap GetSourceWrap(SectionBase section, ITypedItem item) {
    // Figure out the type based on the item or it's parent
    string code = null;
    if (item != null) {
      if (item.String("TutorialType") == "formula")
        return new WrapFormula(section, null);

      if (item.IsNotEmpty("OutputAndSourceDisplay"))
        code = item.String("OutputAndSourceDisplay");
      else {
        var parent = AsItem(item.Parents(type: "TutAccordion"));
        if (parent != null && parent.IsNotEmpty("OutputAndSourceDisplay"))
          code = parent.String("OutputAndSourceDisplay");
      }
    }
    // Default Output over Source
    if (!code.Has() || code == "out-over-src")
      return new WrapOutOverSrc(section);

    // Basic src / out only
    if (code == "src") return new WrapSrcOnly(section);
    if (code == "out") return new WrapOutOnly(section);

    // Split - either a real split, or if width == 0, then 2 tabs
    if (code == "split") {
      if (item.Int("OutputWidth") != 0)
        return new WrapOutSplitSrc(section);
      var wrap = new Wrap(section, null, false);
      wrap.TabSelected = SourceTabName;
      return wrap;
    }
    
    // SourceWrapIntro
    // SourceWrapIntroWithSource
    return new Wrap(section, null, false);
  }

  public class Wrap
  {
    public const string Indent1 = "      ";
    public const string Indent2 = "        ";
    public Wrap(SectionBase sb, string name, bool combined = false) {
      Section = sb;
      Name = name ?? "Wrap";
      Tabs = combined
        ? new List<string> { ResultAndSourceTabName }
        : new List<string> { ResultTabName, SourceTabName };
      TabSelected = combined ? ResultAndSourceTabName : ResultTabName;
      TagCount = new TagCount(Name, true);
    }
    protected readonly SectionBase Section;
    public List<string> Tabs { get; protected set; }
    public string TabSelected {get; set;}
    protected TagCount TagCount;
    protected string Name;
    public virtual ITag OutputOpen() { return Tag.RawHtml(Comment("") + Indent1); }
    public virtual ITag OutputClose() { return Tag.RawHtml(Comment("/")); }

    public virtual ITag SourceOpen() { return Tag.RawHtml(Comment("")); }
    public virtual ITag SourceClose() { return Tag.RawHtml(Comment("/")); }

    protected string Comment(string op, string name = null) {
      return "\n" + Indent1 + "<!-- " + op + (name ?? Name) + " -->\n";
    }
  }

  /// <summary>
  /// Show Output inside a box above Source
  /// </summary>
  internal class WrapOutOverSrc: Wrap
  {
    private const string nameOfClass = "WrapOutOverSrc";
    public WrapOutOverSrc(SectionBase section) : base(section, nameOfClass, true) {
    }


    public override ITag OutputOpen() { return Tag.RawHtml(
      "\n",
      Comment(nameOfClass),
      TagCount.Open(Tag.Div().Data("start", Name).Class("alert alert-info")),
      Tag.H4(ResultTitle)
    ); }

    public override ITag OutputClose() { return Tag.RawHtml(Comment("/"), TagCount.CloseDiv()); }
  }

  internal class WrapOutOnly: Wrap
  {
    private const string nameOfClass = "WrapOutOnly";
    public WrapOutOnly(SectionBase section) : base(section, nameOfClass, true) {
      Tabs = new List<string> { ResultTabName };
      TabSelected = ResultTabName;
    }


    public override ITag OutputOpen() { return Tag.RawHtml(
      "\n",
      Comment(nameOfClass),
      TagCount.Open(Tag.Div().Data("start", Name).Class("alert alert-info")),
      Tag.H4(ResultTitle)
    ); }

    public override ITag OutputClose() { return Tag.RawHtml(Comment("/"), TagCount.CloseDiv()); }
  }

  /// <summary>
  /// Show Source only - output is expected to result in empty HTML
  /// </summary>
  internal class WrapSrcOnly: Wrap
  {
    public WrapSrcOnly(SectionBase section) : base(section, "WrapSrcOnly", true) {
      TabSelected = SourceTabName;
    }

    public override ITag OutputOpen() { return Tag.RawHtml(
      "\n",
      Comment("WrapSrcOnly")
    ); }

    public override ITag OutputClose() { return Tag.RawHtml(Comment("/")); }
  }


  internal class WrapOutSplitSrc: Wrap
  {
    public WrapOutSplitSrc(SectionBase section) : base(section, "WrapOutSplitSrc", true)
    {
      FirstWidth = section.Item.Int("OutputWidth", fallback: 50);
    }
    private int FirstWidth;

    public override ITag OutputOpen() { return Tag.RawHtml(
      Comment("", "Splitter"),
      Indent1,
      TagCount.Open(Tag.Div().Id(Section.TabPrefix + "-splitter").Class("splitter-group")),
      "\n" + Indent2 + "<!-- split left -->",
      "\n" + Indent2,
      TagCount.Open(Tag.Div().Id(Section.TabPrefix + "-splitter-left")),
      "\n" + Indent2,
      Tag.H4("⬇️ " + ResultTitle + " | Source ➡️"),
      "\n" + Indent2,
      TagCount.Open(Tag.Div().Class("alert alert-info").Style("margin-right: 10px;")),
      Indent1
    ); }

    public override ITag OutputClose() { return Tag.RawHtml(
      "\n" + Indent2,
      TagCount.CloseDiv(),
      "\n" + Indent2 + "<!-- /split-left -->"
      + "\n" + Indent2,
      TagCount.CloseDiv(),
      "\n" + Indent2 + "<!-- split-right -->"
      + "\n" + Indent2,
      TagCount.Open(Tag.Div().Id(Section.TabPrefix + "-splitter-right"))
    ); }

    public override ITag SourceClose() {
      // Ensure it's registered in turnOn
      Section.ScParent.Kit.Page.TurnOn("window.splitter.init()", data: new {
        parts = new [] {
          "#" + Section.TabPrefix + "-splitter-left",
          "#" + Section.TabPrefix + "-splitter-right"
        },
        options = new {
          sizes = new [] { FirstWidth, 100 - FirstWidth },
        }
      });

      // Return the closing tags
      return Tag.RawHtml(
        Indent2,
        TagCount.CloseDiv(),
        Indent2 + "<!-- /split-right -->\n"
        + "\n" + Indent1,
        TagCount.CloseDiv(),
        Indent1 + "<!-- /Splitter -->\n"
      );
    }
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
  private ITag ShowSnippet(string snippetId, ITypedItem item = null, string file = null) {
    if (file != null)
      return ShowFileContents(file, snippetId, withIntro: false, showTitle: false, expand: true);
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
    var l = Log.Call<ITag>("file: '" + file + "'; SnippetId: '" + snippetId + "'");
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
      var specs = GetFileAndProcess(file, snippetId);
      path = specs.Path;  // update in case of error
      errPath = debug ? specs.FullPath : path;
      title = title ?? "Source Code of " + (Text.Has(specs.FileName)
        ? titlePath + specs.FileName  // "Source code of .../Fancybox.cs"
        : "this " + specs.Type); // "this snippet" vs "this file"
      specs.Expand = expand ?? specs.Expand;
      specs.Wrap = wrap ?? specs.Wrap;
      specs.ShowIntro = withIntro ?? specs.ShowIntro;
      specs.ShowTitle = showTitle ?? specs.ShowTitle;

      TurnOnSource(specs, specs.FileName, specs.Wrap);

      return l(Tag.RawHtml(
        debug ? Tag.Div(errPath).Class("alert alert-info") : null,
        "\n<!-- Source Code -->\n",
        SourceBlock(specs, title),
        "\n<!-- /Source Code -->\n"
      ), "ok");
    }
    catch
    {
      throw;
      return Tag.Div("Error showing " + errPath).Class("alert alert-warning");
      return l(ShowError(path), "error");
    }
    // return null;
  }

  public bool FileContainsSnipCode(string file) {
    try {
      var specs = GetFileAndProcess(file);
      return specs.Contents.Contains("snip = Sys.SourceCode.");
    } catch {
      return false;
    }
  }

  private SourceInfo GetFileAndProcess(string file, string snippetId = null, string path = null) {
    path = path ?? Path;
    var fullPath = GetFileFullPath(path, file);
    var cacheKey = (fullPath + "#" + snippetId).ToLowerInvariant();
    if (_sourceInfoCache.ContainsKey(cacheKey)) return _sourceInfoCache[cacheKey];
    var fileInfo = GetFile(path, file, fullPath);
    fileInfo.Processed = SourceProcessor.CleanUpSource(fileInfo.Contents, snippetId);
    fileInfo.Size = Size(null, fileInfo.Processed);
    var isSnippet = !string.IsNullOrWhiteSpace(snippetId);
    fileInfo.ShowIntro = !isSnippet;
    fileInfo.ShowTitle = !isSnippet;
    fileInfo.Type = isSnippet ? "snippet" : "file";
    fileInfo.DomAttribute = "source-code-" + MyContext.Module.Id;
    // If no File name was specified, then it's the current file; don't expand
    if (!snippetId.Has() && !file.Has()) fileInfo.Expand = false;
    _sourceInfoCache[cacheKey] = fileInfo;
    return fileInfo;
  }
  private Dictionary<string, SourceInfo> _sourceInfoCache = new Dictionary<string, SourceInfo>();

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
    var l = Log.Call("filePath:" + filePath + ", wrap:" + wrap + "; specs.Language: " + specs.Language);
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
    l("language=" + language);
  }

  /// <summary>
  /// Determine the ace9 language of the file
  /// </summary>
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

  #region (private/internal) File Processing

  private string GetFileFullPath(string filePath, string file) {
    var l = Log.Call<string>("filePath:" + filePath + ", file:" + file);
    if (Text.Has(file)) {
      if (file.IndexOf(".") == -1)
        file = file + ".cshtml";
      var lastSlash = filePath.LastIndexOf("/");
      filePath = filePath.Substring(0, lastSlash) + "/" + file;
    }
    var fullPath = filePath;
    if (filePath.IndexOf(":") == -1 && filePath.IndexOf(@"\\") == -1)
      fullPath = GetFullPath(filePath);
    return l(fullPath, fullPath);
  }

  private SourceInfo GetFile(string filePath, string file, string fullPath = null) {
    var l = Log.Call<SourceInfo>("filePath:" + filePath + ", file:" + file + "; fullPath: " + fullPath);
    fullPath = fullPath ?? GetFileFullPath(filePath, file);
    var cacheKey = fullPath.ToLowerInvariant();
    var contents = (_getFileCache.ContainsKey(cacheKey))
      ? _getFileCache[cacheKey]
      : System.IO.File.ReadAllText(fullPath);
    var fileName = System.IO.Path.GetFileName(fullPath);
    return l(new SourceInfo { FileName = fileName, Path = filePath, FullPath = fullPath, Contents = contents }, fullPath);
  }

  private Dictionary<string, string> _getFileCache = new Dictionary<string, string>();


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
    public string FileName;
    public string Path;
    public string FullPath;
    public string Contents;
  }

  public string GetFullPath(string filePath) {
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

  #endregion

}

/// <summary>
/// Helper class to count tags and add comments
/// Duplicated in Accordion.cs and SourceCode.cs
/// Try to keep in sync
/// </summary>
public class TagCount {
  public TagCount(string name, bool enabled) { Name = name; Enabled = enabled; }
  public string Name; public bool Enabled; public int Count = 0;
  public string Open() { return "\n<!-- opened " + Name + " OpenCount: " + ++Count + " -->\n"; }
  public string Close() { return "<!-- closed " + Name + " OpenCount: " + --Count + " -->\n"; }

  public IHtmlTag Open(IHtmlTag tag) { return Tag.RawHtml(tag.TagStart, Open()); }
  public IHtmlTag Close(string html) { return Tag.RawHtml(html, Close()); }
  public IHtmlTag CloseDiv() { return Close("</div>"); }
}