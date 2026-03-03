using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using System.Collections.Generic;
using AppCode.Data;

// 2sxclint:disable:no-Presentation-in-quotes - it's just used as a css-class below

// Shared / re-used code to create bootstrap tabs

namespace AppCode.TutorialSystem.Tabs
{
  public class BootstrapTabs: Custom.Hybrid.CodeTyped
  {
    private const string Indent = "    ";
    private const string IndentLi = "      ";
    private const string IndentBtn = "        ";

    public ITag TabList(TutorialSnippet item, string prefix, List<TabSpecs> tabs, TabSpecs active) /* string active = null) */ {
      var tabList = new List<object>();
      foreach (var tab in tabs) {
        var isFirst = tabList.Count == 0;
        var isActive = (active == null && isFirst) || tab.DisplayName == active.DisplayName;

        tabList.Add("\n\n" + IndentLi + "<!-- Tab '" + tab.DisplayName + "'-->");
        tabList.Add("\n" + IndentLi);

        // Generate button, and toolbar to edit/create the add-on definition
        var tabLi = TabLi(tab, prefix, isActive);
        if (tab.AddOn != null)
          tabLi = tabLi.Attr(Kit.Toolbar.Edit(tab.AddOn));
        else {
          // Create a toolbar to convert the current code-based tab into an add-on, pre-filling the file path and type
          // But skip for Output-tabs
          if (tab.Type != TabType.Results && tab.Type != TabType.ResultsAndSource && tab.Type != TabType.Source && tab.Type != TabType.TutorialReferences && tab.Type != TabType.InDepth)
          {
            var tlb = Kit.Toolbar.Empty().New(
              item.AddOns,
              tweak: t => {
                t = t.Prefill(nameof(TutorialSnippetAddOn.AddOnType), tab.ToAddOnType());

                if (tab.Type != TabType.ViewConfig)
                  t = t.Prefill(nameof(TutorialSnippetAddOn.FilePath), tab.Value);
                  
                return t;
              }
            );
            tabLi = tabLi.Attr(tlb);
          }
        }
        tabList.Add(tabLi); // first entry is active = true
      }
      return Tag.RawHtml(
        "\n" + Indent + "<!-- TabList Start '" + prefix + "'-->\n",
        Indent,
        Tag.Ul().Class("nav nav-pills p-3 rounded-top border")
          .Attr("role", "tablist")
          .Wrap(tabList),
        "\n" + Indent + "<!-- TabList End '" + prefix + "'-->\n"
      );
    }


    private Li TabLi(TabSpecs tab, string prefix, bool active) {
      return Tag.Li().Class("nav-item").Attr("role", "presentation").Wrap(
        "\n",
        IndentBtn + $"<!-- Tab button '{tab.Original}', type: {tab.Type} -->\n",
        IndentBtn,
        TabButton(prefix, tab.DisplayName, tab.DomId, active),
        "\n" + IndentLi
      );
    }

    private ITag TabButton(string prefix, string title, string name, bool selected) {
      var realId = prefix + name;
      return Tag.Button(title)
        .Class("nav-link " + (selected ? "active" : ""))
        .Id(prefix + "-tab")
        .Attr("data-bs-toggle", "tab")
        .Attr("data-bs-target", "#" + realId)
        .Type("button")
        .Attr("role", "tab")
        .Attr("aria-controls", realId)
        .Attr("aria-selected", selected.ToString().ToLower());
    }

    // private Div TabContentGroup() {
    //   return Tag.Div().Class("tab-content p-3 border border-top-0 bg-light mb-5");
    // }

    public object TabContentGroupOpen() {
      _tabContentGroupIsOpen = true;
      return Tag.RawHtml(
        "\n" + Indent + "<!-- TabContentGroupOpen -->\n",
        Indent,
        Tag.Div().Class("tab-content p-3 border border-top-0 bg-light mb-5").TagStart
      );
    }
    private bool _tabContentGroupIsOpen = false;

    public string TabContentGroupClose() {
      var result = _tabContentGroupIsOpen ? "</div>\n": null;
      _tabContentGroupIsOpen = false;
      return result;
    }

    private Div TabContentDiv(string prefix, string id, bool isActive = false) {
      var realId = prefix + id;
      return Tag.Div()
          .Class("tab-pane fade " + (isActive ? "show active" : ""))
          .Id(realId)
          .Attr("role", "tabpanel")
          .Attr("aria-labelledby", realId + "-tab");
    }

    public string TabContentOpen(string prefix, string id, bool isActive) {
      _tabContentIsOpen = true;
      return "\n" + Indent + "<!-- TabContentOpen -->\n"
        + Indent
        + TabContentDiv(prefix, id, isActive).TagStart
        + "\n";
    }
    private bool _tabContentIsOpen = false;
    public string TabContentClose() {
      if (!_tabContentIsOpen)
        return "\n" + Indent + "<!-- TabContentClose - already closed -->\n";
      _tabContentIsOpen = false;
      var result = _tabContentIsOpen ? "</div>": null;
      return "\n" + Indent + "<!-- TabContentClose -->\n"
        + Indent + "</div>" + "\n";
    }

    public ITag TabContent(string prefix, string id, object result, bool isActive) {
      return Tag.RawHtml(
        "\n" + Indent + "<!-- TabContent '" + prefix +"':'" + id + "' -->\n",
        Indent,
        TabContentDiv(prefix, id, isActive).Wrap(result),
        "\n",
        Indent + "<!-- /TabContent '" + prefix +"':'" + id + "' -->\n"
      );
    }

    // private string[] _moreTabNames;
    // 2023-09-12 2dm disabled, probably don't need any more
    // public string GetTabName(int index) {
    //   var l = Log.Call<string>("index:" + index);
    //   if (_moreTabNames == null || !_moreTabNames.Any()) return l("no names", "unknown");
    //   if (_moreTabNames.Length < index + 1) return l("index to high", "unknown");
    //   var name = _moreTabNames[index];
    //   Log.Add("name before optimization: '" + name + "'");
    //   return l(name, name);
    // }
  }
}