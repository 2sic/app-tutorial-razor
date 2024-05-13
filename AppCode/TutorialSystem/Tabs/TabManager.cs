using ToSic.Razor.Blade;
using ToSic.Sxc.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using AppCode.TutorialSystem.Wrappers;
using AppCode.TutorialSystem.Source;
using AppCode.Data;

namespace AppCode.TutorialSystem.Tabs
{
  public class TabManager
  {
    public TabManager(SourceCode scParent, TutorialSnippet item, List<TabSpecs> tabSpecs, Wrap sourceWrap) {
      ScParent = scParent;
      Log = ScParent.Log;
      Item = item;
      TabSpecs = tabSpecs ?? new List<TabSpecs>();
      SourceWrap = sourceWrap;
      // ActiveTabName = sourceWrap.TabSelected;
    }

    public readonly List<TabSpecs> TabSpecs;
    public bool HasTab(string tabName) => TabSpecs.Any(t => t.Label.Equals(tabName, StringComparison.InvariantCultureIgnoreCase));
    public bool HasTab(TabType type) => TabSpecs.Any(t => t.Type == type);

    protected readonly SourceCode ScParent; // also used in Formulas
    private TutorialSnippet Item { get; set; }
    // public string ActiveTabName;
    private readonly Wrap SourceWrap;
    public TabSpecs ActiveTab => SourceWrap.TabSpecSelected;
    private readonly ICodeLog Log;

    #region TabNames

    public List<TabSpecs> CompleteTabs => _completeTabs ??= GetFinalTabs();
    private List<TabSpecs> _completeTabs;

    private List<TabSpecs> GetFinalTabs()
    {
      // Logging
      var l = Log.Call<List<TabSpecs>>();
      // Build list
      var list = new List<TabSpecs>();

      // If we have source-wrap, that determines what tabs to add
      if (SourceWrap != null)
      {
        Log.Add("SourceWrap: " + SourceWrap);
        list.AddRange(SourceWrap.TabSpecs);
      }

      if (TabSpecs.Any())
      {
        Log.Add("Tab Count: " + TabSpecs.Count());
        Log.Add("Replace Tab Count: " + _replaceTabContents?.Count());
        var tabsToAdd = TabSpecs;
        if (_replaceTabContents != null)
          for (var i = 0; i < _replaceTabContents.Count(); i++)
          {
            var replace = _replaceTabContents[i];
            var tab = tabsToAdd[i];
            if (tab != null) tab.Body = replace;
          }
        list.AddRange(tabsToAdd);
      }
      // Else custom tabs in configuration
      else if (Item.IsNotEmpty("Tabs"))
      {
        // TODO: 2dm - atm only used on 3 formulas, all which have files
        // but should be changed to properly detect and set the type based on that
        Log.Add("Tabs: " + Item.Tabs);
        var addTabs = Item.Tabs.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
          .Select(t => t.Trim())
          .Select(t => new TabSpecs(TabType.File, t));
        list.AddRange(addTabs);
      }

      // If we don't have an item, then finish now
      if (Item == null)
        return l(list, list.Count.ToString());

      // Otherwise add some default tabs based on the data available
      AddIfFound(nameof(Item.InDepthExplanation), () => TabSpecsFactory.InDepth());
      AddIfFound(nameof(Item.Notes), () => TabSpecsFactory.Notes());
      AddIfFound(nameof(Item.Tutorials), () => TabSpecsFactory.TutorialsRef());

      return l(list, list.Count.ToString());

      void AddIfFound(string key, Func<TabSpecs> factory)
      {
        if (Item.IsEmpty(key)) return;
        Log.Add(key);
        list.Add(factory());
      }
    }

    #endregion

    #region TabContents

    public void ReplaceTabContents(List<object> replacement) {
      var l = Log.Call("replacement: " + (replacement == null ? "null" : "" + replacement.Count()));
      _replaceTabContents = replacement;
      _completeTabs = null;  // Reset so it will be regenerated
      l("ok");
    }
    private List<object> _replaceTabContents;


    #endregion

    #region Debug

    public string TabNamesDebug => string.Join(", ", CompleteTabs.Select(tc => Text.Ellipsis(tc.DisplayName, 20)));
    public string TabContentsDebug => string.Join(", ", CompleteTabs.Select(tc => Text.Ellipsis(tc.Body?.ToString() ?? "", 20)));

    #endregion

  }
}