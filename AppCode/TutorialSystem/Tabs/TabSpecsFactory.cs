using ToSic.Eav.Data;
using ToSic.Razor.Blade;
using System.Collections.Generic;
using System.Linq;
using AppCode.TutorialSystem.Source;

namespace AppCode.TutorialSystem.Tabs
{
  /// <summary>
  /// Helper to build TabSpecs objects from a file source code or string
  /// </summary>
  public class TabSpecsFactory: AppCode.Services.ServiceBase
  {
    #region Dependencies: File Handler

    private FileHandler FileHandler => _fileHandler ??= GetService<FileHandler>();
    private FileHandler _fileHandler;

    #endregion

    #region Static Create Standard TabSpecs

    internal static TabSpecs InDepth(string label = null) => new TabSpecs(TabType.InDepth, label ?? Constants.InDepthTabName);

    internal static TabSpecs Notes(string label = null) => new TabSpecs(TabType.Notes, label ?? Constants.NotesTabName);

    internal static TabSpecs TutorialsRef(string label = null) => new TabSpecs(TabType.TutorialReferences, label ?? Constants.TutorialsTabName);

    internal static TabSpecs Results(string label = null) => new TabSpecs(TabType.Results, label ?? Constants.ResultTabName);

    internal static TabSpecs Source(string label = null) => new TabSpecs(TabType.Source, label ?? Constants.SourceTabName);
    internal static TabSpecs ResultsAndSource(string label = null) => new TabSpecs(TabType.ResultsAndSource, label ?? Constants.ResultAndSourceTabName);

    internal static TabSpecs ViewConfig(string label = null) => new TabSpecs(TabType.ViewConfig, label?? Constants.ViewConfigTabName);

    internal static TabSpecs Formulas(string label = null) => new TabSpecs(TabType.Formulas, label ?? Constants.FormulasTabName);

    #endregion

    internal List<TabSpecs> TryToGetTabsFromSource(string codeFilePath)
    {
      if (!codeFilePath.Has() || codeFilePath == Constants.IgnoreSourceFile)
        return new List<TabSpecs>();

      // Get the source code, find out if it contains anything
      var srcPath = codeFilePath.Replace("\\", "/").BeforeLast("/");
      var src = FileHandler.GetFileAndProcess(codeFilePath).Contents;
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
          var (found, label, value) = SplitTabEntry(entry, "file:");
          if (found)
          {
            // the path could be "/AppCode/..." or it could be (older) "../../something"
            var finalPath = value.StartsWith("/") ? value: srcPath + "/" + value;

            label = label != ""
              ? label
              : finalPath.Contains("/AppCode/") ? finalPath : null;

            Log.Add($"Label: '{label}'; Custom Label: '{label}'; finalPath: '{finalPath}'");
            return new TabSpecs(TabType.File, label ?? "file:" + finalPath, value: "file:" + finalPath, original: t);
          }
          
          (found, label, value) = SplitTabEntry(entry, "model:");
          if (found)
            return new TabSpecs(TabType.Model, label: label != "" ? label : $"Model: {value}.cs", value: value, original: t);
          
          (found, label, value) = SplitTabEntry(entry, "datasource:");
          if (found)
            return new TabSpecs(TabType.DataSource, label: label != "" ? label : $"DataSource: {value}.cs", value: value, original: t);

          // Final - none of the special cases
          return SplitStringToTabSpecs(t);
        })
        .ToList();
      return tabs;

      static (bool found, string label, string value) SplitTabEntry(string entry, string prefix)
      {
        if (!entry.Contains(prefix)) return (false, null, null);
        return (true, Text.Before(entry, prefix), Text.After(entry, prefix));
      }
    }

    /// <summary>
    /// Split a string such as "label|value" into a TabSpecs object
    /// If it doesn't have a label, it will use the value as label
    /// </summary>
    private TabSpecs SplitStringToTabSpecs(string tabString)
    {
      var pair = tabString.Trim().Split('|');
      var hasLabel = pair.Length > 1;
      var pVal = hasLabel ? pair[1] : pair[0];
      var pLabel = pair[0];

      // Figure out the parts
      var label = (hasLabel ? pLabel : tabString).Trim(); 
      var value = pVal.Trim();
      Log.Add("Tab Entry: " + label + " = " + value);

      // Special cases
      var specialCaseLabel = hasLabel ? pLabel : null;
      if (pVal.Equals(Constants.ViewConfigCode))
        return ViewConfig(specialCaseLabel);

      // This is probably never in use, as it shows data in the item
      // so it never makes sense to specify it in the source code
      // if (pVal.Equals(Constants.InDepthCode))
      //   return InDepth(specialCaseLabel);

      // if (pVal.Equals(Constants.NotesCode))
      //   return Notes(specialCaseLabel);

      // throw new System.Exception($"Tab '{label}', '{pVal}' not found - make sure the view has this");

      // These are tabs which just show the name, the body will be filled by the code
      return new TabSpecs(TabType.FromCode, label: label, value: value, original: tabString);
    }



  }
}