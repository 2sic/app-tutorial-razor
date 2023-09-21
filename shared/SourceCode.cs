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
    BsTabs = GetCode("BootstrapTabs.cs");
    return this;
  }
  public TagCount TagCount = new TagCount("SourceCode", true);
  public dynamic Sys {get;set;}
  public string Path { get; set; }
  private dynamic BsTabs {get;set;}

  public dynamic Formulas { get { return _formulas ?? (_formulas = GetCode("SourceCodeFormulas.cs")).Init(this); } }
  private dynamic _formulas;

  public dynamic FileHandler { get { return _fileHandler ?? (_fileHandler = GetCode("FileHandler.cs")).Init(Path); } }
  private dynamic _fileHandler;

  #endregion

  #region All Available Entry Points to Show Snippets in Various Ways

  /// <summary>
  /// QuickRef section - only to be used in the Quick Reference
  /// for manual adding complex cases - ATM not in use 2023-09-12
  /// </summary>
  /// <param name="item">ATM object, because when coming through a Razor14 the type is not known</param>
  /// <param name="tabs"></param>
  /// <returns></returns>
  public TutorialSection QuickRef(object item, string tabs = null, Dictionary<string, string> tabDic = null)
  {
    var l = Log.Call<TutorialSection>("tabs: '" + tabs + "'");
    tabDic = tabDic ?? TabStringToDic(tabs);
    var result = new TutorialSection(this, item as ITypedItem, tabDic);
    return l(result, "ok - count: " + tabDic.Count());
  }

  /// <summary>
  /// Create a Snip/Section object from an Item (ITypedItem).
  /// Only used in Accordion
  /// </summary>
  /// <param name="item">The configuration item</param>
  /// <param name="file">The file from which it will be relative to</param>
  /// <returns></returns>
  public TutorialSection SnipFromItem(ITypedItem item, string file = null)
  {
    var l = Log.Call<TutorialSection>("file: " + file);
    // If we have a file, we should try to look up the tabs
    var tabCsv = TryToGetTabsFromSource(file);
    Log.Add("tabs: '" + tabCsv + "'");
    var tabs = TabStringToDic(tabCsv);
    var result = new TutorialSection(this, item, tabs, sourceFile: file);
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
        var label = hasLabel ? pLabel : t; 
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
    var src = FileHandler.GetFileContents(file) as string;
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


  #region SectionBase

  /// <summary>
  /// Base class for all code sections with snippets etc.
  /// </summary>
  public class TutorialSection {
    public TutorialSection(SourceCode sourceCode, ITypedItem item, Dictionary<string, string> tabs, string sourceFile = null) {
      if (item == null) throw new Exception("Item should never be null");
      ScParent = sourceCode;
      BsTabs = ScParent.BsTabs;
      Log = ScParent.Log;
      Item = item;
      SourceWrap = sourceCode.GetSourceWrap(this, item);
      TabHandler = new TabManager(sourceCode, item, tabs, sourceWrap: SourceWrap);
      SourceFile = sourceFile;
      ViewConfig = ScParent.GetCode("./ViewConfigurationSimulation.cs").Setup(this.TabHandler);
    }
    internal SourceCode ScParent;
    private dynamic BsTabs;
    internal ICodeLog Log;
    protected int SnippetCount;
    internal string SnippetId {get; private set;}
    public string TabPrefix {get; protected set;}
    protected Wrap SourceWrap;
    internal TabManager TabHandler;
    internal ITypedItem Item;
    internal string SourceFile;
    public dynamic ViewConfig;

    /// <summary>
    /// The SnippetId must be provided here, so it can be found in the source code later on
    /// </summary>
    public ITag SnipStart(string snippetId = null) {
      InitSnippetAndTabId(snippetId);
      return TabsBeforeContent();
    }

    /// <summary>
    /// Helper which is usually called by implementations of SnipStart
    /// </summary>
    protected void InitSnippetAndTabId(string snippetId = null) {
      SnippetCount = ScParent.SourceCodeTabCount++;
      SnippetId = snippetId ?? "" + SnippetCount;
      TabPrefix = "tab-" + ScParent.UniqueKey + "-" + SnippetCount + "-" + (snippetId ?? "auto-id");
    }

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

    /// <summary>
    /// End of Snippet, so the source-analyzer can find it.
    /// Usually overridden by the implementation.
    /// </summary>
    public virtual ITag SnipEnd()  { return SnipEndFinal(); }

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
            ScParent.FileHandler.ShowSnippet(SnippetId, item: Item, file: SourceFile),
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
            ScParent.FileHandler.ShowSnippet(SnippetId, item: Item, file: SourceFile),
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
        return ScParent.FileHandler.GetTabFileContents(strResult.Substring(5));

      // Handle case "html-img:..."
      if (strResult.StartsWith("html-img:"))
        return ScParent.FileHandler.ShowResultImg(strResult.Substring(9));

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


  public class TabManager
  {
    public TabManager(SourceCode scParent, ITypedItem item, Dictionary<string, string> tabs, Wrap sourceWrap = null) {
      ScParent = scParent;
      Log = ScParent.Log;
      Item = item;
      Tabs = tabs ?? new Dictionary<string, string>();
      SourceWrap = sourceWrap;
      ActiveTabName = sourceWrap.TabSelected;
    }
    public readonly Dictionary<string, string> Tabs;
    protected readonly SourceCode ScParent; // also used in Formulas
    private ITypedItem Item { get; set; }
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
        .Select(n => {
          if (n == ViewConfigCode) return ViewConfigTabName;
          if (n.EndsWith(".csv.txt")) return n.Replace(".csv.txt", ".csv");
          if (n == InDepthField) return InDepthTabName;
          if (n.StartsWith("file:")) return Text.AfterLast(n, "/") ?? Text.AfterLast(n, ":");
          return n;
        })
        .ToList();
      return l(names, names.Count.ToString()); 
    }

    private List<object> GetTabNamesAndContentBase(bool useKeys) {
      // Logging
      var l = Log.Call<List<object>>();
      // Build list
      var list = new List<object>();

      // If we have source-wrap, that determines what tabs to add
      if (this.SourceWrap != null) {
        Log.Add("SourceWrap: " + SourceWrap);
        list.AddRange(SourceWrap.Tabs);
      }        

      if (Tabs != null && Tabs.Any()) {
        Log.Add("Tab Count: " + Tabs.Keys.Count());
        if (useKeys) list.AddRange(Tabs.Keys);
        else {
          if (_replaceTabContents != null) {
            list.AddRange(_replaceTabContents);
            // If the tabs have more entries - eg "ViewConfiguration" - then add that at the end as well
            if (Tabs.Values.Count() > _replaceTabContents.Count())
              list.AddRange(Tabs.Values.Skip(_replaceTabContents.Count()));
          }
          else list.AddRange(Tabs.Values);
        }
        // Else custom tabs in configuration
      } else if (Item.IsNotEmpty("Tabs")) {
        Log.Add("Tabs: " + Item.String("Tabs"));
        list.AddRange(Item.String("Tabs").Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(t => t.Trim()));
      }

      if (Item != null) {
        if (Item.IsNotEmpty(InDepthField)) {
          Log.Add(InDepthField);
          list.Add(InDepthTabName);
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

  #region Counter / Identifiers

  /// <summary>
  /// Count of source code snippets - used to create unique IDs
  /// </summary>
  public int SourceCodeTabCount = 0;

  public string UniqueKey { get { return _uniqueKey ?? (_uniqueKey = Kit.Key.UniqueKeyWith(this)); }}
  private string _uniqueKey;

  #endregion

  #region Wrap: Source Wrappers like Wrap, WrapOutOverSrc, WrapOutOnly, WrapSrcOnly, WrapOutSplitSrc, WrapFormula

  private Wrap GetSourceWrap(TutorialSection section, ITypedItem item) {
    // Figure out the type based on the item or it's parent
    string code = null;
    if (item != null) {
      if (item.String("TutorialType") == "formula")
        return new WrapFormula(section);

      if (item.IsNotEmpty("OutputAndSourceDisplay"))
        code = item.String("OutputAndSourceDisplay");
      else {
        var parent = AsItem(item.Parents(type: "TutorialGroup"));
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
    public Wrap(TutorialSection sb, string name, bool combined = false) {
      Section = sb;
      Name = name ?? "Wrap";
      Tabs = combined
        ? new List<string> { ResultAndSourceTabName }
        : new List<string> { ResultTabName, SourceTabName };
      TabSelected = combined ? ResultAndSourceTabName : ResultTabName;
      TagCount = new TagCount(Name, true);
    }
    protected readonly TutorialSection Section;
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
    public WrapOutOverSrc(TutorialSection section) : base(section, nameOfClass, true) {
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
    public WrapOutOnly(TutorialSection section) : base(section, nameOfClass, true) {
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
    public WrapSrcOnly(TutorialSection section) : base(section, "WrapSrcOnly", true) {
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
    public WrapOutSplitSrc(TutorialSection section) : base(section, "WrapOutSplitSrc", true)
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

  internal class WrapFormula: Wrap
  {
    public WrapFormula(TutorialSection sb, ITypedItem specs = null) : base(sb, "WrapFormula") {
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