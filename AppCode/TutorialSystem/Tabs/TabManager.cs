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
    public TabManager(SourceCode scParent, TutorialSnippet item, List<TabSpecs> tabSpecs, Wrap sourceWrap = null) {
      ScParent = scParent;
      Log = ScParent.Log;
      Item = item;
      TabSpecs = tabSpecs ?? new List<TabSpecs>();
      SourceWrap = sourceWrap;
      ActiveTabName = sourceWrap.TabSelected;
    }

    public readonly List<TabSpecs> TabSpecs;
    public bool HasTab(string tabName) => TabSpecs.Any(t => t.Label.Equals(tabName, StringComparison.InvariantCultureIgnoreCase));

    protected readonly SourceCode ScParent; // also used in Formulas
    private TutorialSnippet Item { get; set; }
    public string ActiveTabName;
    private readonly Wrap SourceWrap;
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
        list.AddRange(SourceWrap.Tabs.Select(t => new TabSpecs("wrap", t)));
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
        Log.Add("Tabs: " + Item.Tabs);
        var addTabs = Item.Tabs.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
          .Select(t => t.Trim())
          .Select(t => new TabSpecs("config", t));
        list.AddRange(addTabs);
      }

      // If we don't have an item, then finish now
      if (Item == null)
        return l(list, list.Count.ToString());

      // Otherwise add some default tabs as needed
      if (Item.IsNotEmpty(Constants.InDepthField))
      {
        Log.Add(Constants.InDepthField);
        list.Add(new TabSpecs("predefined", Constants.InDepthTabName));
      }
      if (Item.IsNotEmpty(Constants.NotesFieldName))
      {
        Log.Add(Constants.NotesFieldName);
        list.Add(new TabSpecs("predefined", Constants.NotesTabName));
      }
      // TODO: OLD / NEW
      if (Item.IsNotEmpty("Tutorials"))
      {
        Log.Add("Tutorials");
        list.Add(new TabSpecs("predefined", Constants.TutorialsTabName));
      }

      return l(list, list.Count.ToString());
    }

    #endregion

    #region TabContents

    public void ReplaceTabContents(List<object> replacement) {
      var l = Log.Call("replacement: " + (replacement == null ? "null" : "" + replacement.Count()));
      _replaceTabContents = replacement;
      _completeTabs = null;  // Reset so it will be regenerated
      // _tabLabels = null;  // Reset so it will be regenerated
      l("ok");
    }
    private List<object> _replaceTabContents;


    #endregion

    #region Debug

    public string TabNamesDebug => string.Join(", ", CompleteTabs.Select(tc => Text.Ellipsis(tc.DisplayName /*.NiceName()*/, 20)));
    public string TabContentsDebug => string.Join(", ", CompleteTabs.Select(tc => Text.Ellipsis(tc.Contents.ToString() ?? "", 20)));

    #endregion

    #region Archived Code 2024-05-12 when simplifying tabs initialization etc. - remove ca. 2027-07


    // public List<string> TabNames => _tabNames ??= GetTabNames();
    // private List<string> _tabNames;
    // private List<string> GetTabNames() {
    //   // Logging
    //   var l = Log.Call<List<string>>();

    //   // Build Names
    //   var names = GetTabLabelsOrContent(getLabels: true)
    //     .Select(s => s as string)
    //     .Where(s => s != null)
    //     .Select(n => {
    //       // If a known tab identifier, return the nice name
    //       if (n == Constants.ViewConfigCode) return Constants.ViewConfigTabName;
    //       if (n == Constants.InDepthField) return Constants.InDepthTabName;
    //       // if a file, return the file name only (and on csv, fix a workaround to ensure import/export)
    //       if (n.EndsWith(".csv.txt")) n = n.Replace(".csv.txt", ".csv");
    //       if (n.StartsWith("file:")) return Text.AfterLast(n, "/") ?? Text.AfterLast(n, ":");
    //       return n;
    //     })
    //     .ToList();
    //   return l(names, names.Count.ToString()); 
    // }


    // private List<object> GetTabLabelsOrContent(bool getLabels)
    // {
    //   // Logging
    //   var l = Log.Call<List<object>>();
    //   // Build list
    //   var list = new List<object>();

    //   // If we have source-wrap, that determines what tabs to add
    //   if (SourceWrap != null)
    //   {
    //     Log.Add("SourceWrap: " + SourceWrap);
    //     list.AddRange(SourceWrap.Tabs);
    //   }

    //   if (TabSpecs.Any())
    //   {
    //     Log.Add("Tab Count: " + TabSpecs.Count());
    //     if (getLabels)
    //       list.AddRange(TabSpecs.Select(t => t.Label));
    //     else
    //     {
    //       var values = TabSpecs.Select(t => t.Contents).ToList();
    //       if (_replaceTabContents != null)
    //       {
    //         list.AddRange(_replaceTabContents);
    //         // If the tabs have more entries - eg "ViewConfiguration" - then add that at the end as well
    //         if (TabSpecs.Count() > _replaceTabContents.Count())
    //           list.AddRange(values.Skip(_replaceTabContents.Count()));
    //       }
    //       else
    //       {
    //         Log.Add("Add all Tab Values: " + string.Join(",", values));
    //         list.AddRange(values);
    //       }
    //     }
    //   }
    //   // Else custom tabs in configuration
    //   else if (Item.IsNotEmpty("Tabs"))
    //   {
    //     Log.Add("Tabs: " + Item.Tabs);
    //     list.AddRange(Item.Tabs.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(t => t.Trim()));
    //   }

    //   // If we don't have an item, then finish now
    //   if (Item == null)
    //     return l(list, list.Count.ToString());

    //   // Otherwise add some default tabs as needed
    //   if (Item.IsNotEmpty(Constants.InDepthField))
    //   {
    //     Log.Add(Constants.InDepthField);
    //     list.Add(Constants.InDepthTabName);
    //   }
    //   if (Item.IsNotEmpty(Constants.NotesFieldName))
    //   {
    //     Log.Add(Constants.NotesFieldName);
    //     list.Add(Constants.NotesTabName);
    //   }
    //   // TODO: OLD / NEW
    //   if (Item.IsNotEmpty("Tutorials"))
    //   {
    //     Log.Add("Tutorials");
    //     list.Add(Constants.TutorialsTabName);
    //   }

    //   return l(list, list.Count.ToString());
    // }

    // /// <summary>
    // /// The TabContents is either automatic, or set by the caller.
    // /// 
    // /// In many cases it's just a reference like "file:abc.cshtml" which will be resolved when showing.
    // /// </summary>
    // public List<object> TabContents => _tabLabels ??= GetTabContents();
    // private List<object> _tabLabels;

    // private List<object> GetTabContents()
    // {
    //   var l = Log.Call<List<object>>();
    //   var list = GetTabLabelsOrContent(getLabels: false);
    //   return l(list, "tabContents: " + list.Count);
    // }

    // public string TabContentsDebug => string.Join(", ", TabContents.Select(tc => Text.Ellipsis(tc.ToString() ?? "", 20)));
    //  public string TabNamesDebug => string.Join(", ", TabNames);


    #endregion

  }


}