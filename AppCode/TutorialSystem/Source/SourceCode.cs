using ToSic.Eav.Data;
using ToSic.Razor.Blade;
using System.Collections.Generic;
using System.Linq;
using AppCode.TutorialSystem.Wrappers;
using AppCode.TutorialSystem.Sections;
using AppCode.Data;
using AppCode.TutorialSystem.Tabs;

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
      foreach (var tab in tabCsv)
        Log.Add("tabs: '" + tab + "'");
      Log.Add("tabs: '" + tabCsv + "'");
      var result = GetService<TutorialSectionEngine>().Init(this, item, tabCsv, sourceFile: file);
      return l(result, "ok - count: " + tabCsv.Count());
    }

    private List<TabSpecs> TryToGetTabsFromSource(string file)
    {
      if (!file.Has() || file == Constants.IgnoreSourceFile)
        return new List<TabSpecs>();

      // Get the source code, find out if it contains anything
      var srcPath = file.Replace("\\", "/").BeforeLast("/");
      var src = FileHandler.GetFileAndProcess(file).Contents;
      if (!src.Contains("Tut.Tabs="))
        return new List<TabSpecs>();

      // Make sure there is actually something in the config
      var tabsLine = Text.After(src, "Tut.Tabs=");
      var tabsBeforeEol = Text.Before(tabsLine, "\n");
      var tabsString = Text.Before(tabsBeforeEol, "*/") ?? tabsBeforeEol;
      if (!tabsString.Has())
        return new List<TabSpecs>();

      // Generate the TabSpecs
      var tabs = tabsString.Split(',').Select(t =>
        {
          var entry = t.Trim();
          if (entry.Contains("file:")) {
            var prefix = Text.Before(entry, "file:");
            var fullPath = Text.After(entry, "file:");

            // the path could be "/AppCode/..." or it could be (older) "../../something"
            var finalPath = fullPath.StartsWith("/")
              ? fullPath
              : srcPath + "/" + fullPath;

            var label = prefix != ""
              ? prefix
              : !finalPath.Contains("/AppCode/")
                ? null
                : $"{finalPath}";

            Log.Add($"Prefix: '{prefix}'; Custom Label: '{label}'; finalPath: '{finalPath}'");
            return new TabSpecs("file", label ?? "file:" + finalPath, value: "file:" + finalPath, original: t);
          }
          
          if (entry.Contains("model:")) {
            var label = Text.Before(entry, "model:");
            var name = Text.After(entry, "model:");
            var modPath = $"file:/AppCode/Data/{name}.Generated.cs";
            var domIdProbably = modPath;
            return new TabSpecs("model", label: label != "" ? label : $"Model: {name}.cs", value: modPath, original: t);
          }
          
          if (entry.Contains("datasource:")) {
            var label = Text.Before(entry, "datasource:");
            var name = Text.After(entry, "datasource:");
            var dsPath = $"file:/AppCode/DataSources/{name}.cs";
            return new TabSpecs("datasource", label: label != "" ? label : $"DataSource: {name}.cs", value: dsPath, original: t);
          }

          // Final - none of the special cases
          return SplitStringToTabSpecs(t);
        })
        .ToList();
      return tabs;
    }

    /// <summary>
    /// Split a string such as "label|value" into a TabSpecs object
    /// If it doesn't have a label, it will use the value as label
    /// </summary>
    private TabSpecs SplitStringToTabSpecs(string tabString) {
      var pair = tabString.Trim().Split('|');
      var hasLabel = pair.Length > 1;
      var pVal = hasLabel ? pair[1] : pair[0];
      var pLabel = pair[0];

      // Figure out the parts
      var label = (hasLabel ? pLabel : tabString).Trim(); 
      var value = pVal.Trim();
      Log.Add("Tab Entry: " + label + " = " + value);
      return new TabSpecs("string", label: label, value: value, original: tabString);
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