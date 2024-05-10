using ToSic.Eav.Data;
using ToSic.Razor.Blade;
using System.Collections.Generic;
using System.Linq;
using AppCode.TutorialSystem.Wrappers;
using AppCode.TutorialSystem.Sections;
using AppCode.Data;

namespace AppCode.TutorialSystem.Source
{
  public class SourceCode: AppCode.Services.ServiceBase
  {
    #region Init / Dependencies

    private FileHandler FileHandler => _fileHandler ??= GetService<FileHandler>();
    private FileHandler _fileHandler;

    #endregion

    #region All Available Entry Points to Show Snippets in Various Ways

    /// <summary>
    /// Create a Snip/Section object from a TutorialSnippet.
    /// Only used in Accordion
    /// </summary>
    /// <param name="item">The configuration item</param>
    /// <param name="file">The file from which it will be relative to</param>
    /// <returns></returns>
    public TutorialSectionEngine SnipFromItem(TutorialSnippet item, string file = null)
    {
      var l = Log.Call<TutorialSectionEngine>("file: " + file);
      // If we have a file, we should try to look up the tabs
      Log.Add("tabs before:" + file);
      var tabCsv = TryToGetTabsFromSource(file);
      Log.Add("tabs: '" + tabCsv + "'");
      var tabs = TabStringToDic(tabCsv);
      var result = GetService<TutorialSectionEngine>().Init(this, item, tabs, sourceFile: file);
      return l(result, "ok - count: " + tabs.Count());
    }


    private Dictionary<string, string> TabStringToDic(string[] tabs) {
      var tabList = (tabs ?? new string[0]).Select(t => t.Trim()).ToArray();
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
          Log.Add("Tab Entry: " + label + " = " + value);
          return new {
            label,
            value,
            original = t
          };
        })
        .ToDictionary(t => t.label, t => t.value);
      return tabDic;
    }

    private string[] TryToGetTabsFromSource(string file)
    {
      if (!file.Has() || file == Constants.IgnoreSourceFile) return null;
      var srcPath = file.Replace("\\", "/").BeforeLast("/");
      var src = FileHandler.GetFileAndProcess(file).Contents;
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
          string prefix;
          string fullPath;
          if (entry.Contains("file:")) {
            prefix = Text.Before(entry, "file:");
            fullPath = Text.After(entry, "file:");
          }
          else if (entry.Contains("model:")) {
            prefix = Text.Before(entry, "model:");
            fullPath = "/AppCode/Data/" + Text.After(entry, "model:") + ".Generated.cs";
          }
          else if (entry.Contains("datasource:")) {
            prefix = Text.Before(entry, "datasource:");
            fullPath = "/AppCode/DataSources/" + Text.After(entry, "datasource:") + ".cs";
          }
          else
            return t;

          // if (!entry.Contains("file:")) return t;
          // the path could be "/AppCode/..." or it could be (older) "../../something"
          var finalPath = fullPath.StartsWith("/") ? fullPath : srcPath + "/" + fullPath;
          var customLabel = prefix == "" && !finalPath.Contains("/AppCode/") ? "" : $"{finalPath}|";
          Log.Add($"Prefix: '{prefix}'; Custom Label: '{customLabel}'; finalPath: '{finalPath}'");
          prefix = customLabel;
          return prefix + "file:" + finalPath;
        })
        .ToArray();
      return tabs;
    }

    #endregion


    #region Counter / Identifiers

    /// <summary>
    /// Count of source code snippets - used to create unique IDs
    /// </summary>
    public int SourceCodeTabCount = 0;

    #endregion

    #region Wrap: Source Wrappers like Wrap, WrapOutOverSrc, WrapOutOnly, WrapSrcOnly, WrapOutSplitSrc, WrapFormula

    internal Wrap GetSourceWrap(TutorialSectionEngine section, TutorialSnippet item) {
      // Figure out the type based on the item or it's parent
      string code = null;
      if (item != null) {
        if (item.TutorialType == "formula")
          return new WrapFormula(section);

        if (item.IsNotEmpty(nameof(item.OutputAndSourceDisplay)))
          code = item.OutputAndSourceDisplay;
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
        var wrap = new Wrap(section, "WrapInsteadOfSplit")
        {
          TabSelected = Constants.SourceTabName
        };
        return wrap;
      }
      
      // SourceWrapIntro
      // SourceWrapIntroWithSource
      return new Wrap(section, "UnknownWrapperAutoDefault");
    }

    #endregion

  }
}