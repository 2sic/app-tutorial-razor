using ToSic.Eav.Data;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using System.Collections.Generic;
using System.Linq;
using AppCode.Tutorial;
using AppCode.Wrappers;

namespace AppCode.Source
{
  public class SourceCode: Custom.Hybrid.CodeTyped
  {
    #region Init / Dependencies

    public SourceCode Init(Sys sys, string path) {
      Sys = sys;
      Path = path;
      BsTabs = GetService<BootstrapTabs>();
      return this;
    }
    public TagCount TagCount = new TagCount("SourceCode", true);
    public Sys Sys {get;set;}
    public string Path { get; set; }
    internal BootstrapTabs BsTabs {get;set;}

    public SourceCodeFormulas Formulas => _formulas ??= GetService<SourceCodeFormulas>().Init(this);
    private SourceCodeFormulas _formulas;

    public FileHandler FileHandler => _fileHandler ??= GetService<FileHandler>().Init(Path);
    private FileHandler _fileHandler;

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

    private string TryToGetTabsFromSource(string file)
    {
      if (!file.Has() || file == Constants.IgnoreSourceFile) return null;
      var srcPath = file.Replace("\\", "/").BeforeLast("/");
      var src = FileHandler.GetFileContents(file);
      if (!src.Contains("Tut.Tabs="))
        return null;

      var tabsLine = Text.After(src, "Tut.Tabs=");
      var tabsBeforeEol = Text.Before(tabsLine, "\n");
      var tabsString = Text.Before(tabsBeforeEol, "*/") ?? tabsBeforeEol;
      if (!tabsString.Has()) return null;
      var tabs = tabsString.Split(',')
        .Select(t =>
        {
          var entry = t.Trim();
          if (!entry.Contains("file:")) return t;
          var prefix = Text.Before(entry, "file:");
          var fileName = Text.After(entry, "file:");
          return prefix + "file:" + srcPath + "/" + fileName;
        })
        .ToArray();
      var result = string.Join(",", tabs);
      return result;
    }

    #endregion


    #region Counter / Identifiers

    /// <summary>
    /// Count of source code snippets - used to create unique IDs
    /// </summary>
    public int SourceCodeTabCount = 0;

    public new string UniqueKey => _uniqueKey ??= Kit.Key.UniqueKeyWith(this);
    private string _uniqueKey;

    #endregion

    #region Wrap: Source Wrappers like Wrap, WrapOutOverSrc, WrapOutOnly, WrapSrcOnly, WrapOutSplitSrc, WrapFormula

    internal Wrap GetSourceWrap(TutorialSection section, ITypedItem item) {
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
        var wrap = new Wrap(section, "WrapInsteadOfSplit");
        wrap.TabSelected = Constants.SourceTabName;
        return wrap;
      }
      
      // SourceWrapIntro
      // SourceWrapIntroWithSource
      return new Wrap(section, "UnknownWrapperAutoDefault");
    }

    #endregion

  }
}