using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// Shared / re-used code to create bootstrap tabs
public class BootstrapTabs: Custom.Hybrid.Code14
{
  public ITag TabList(string prefix, IEnumerable<string> names) {
    // Remember tab names
    _moreTabNames = names.ToArray();
    var tabList = new List<ITag>();
    foreach(var name in names)
      tabList.Add(Tab(prefix, name, tabList.Count == 0)); // first entry is active = true
    return Tag.Ul().Class("nav nav-pills p-3 rounded-top border").Attr("role", "tablist").Wrap(tabList);
  }

  // WARNING: DUPLICATE CODE
  private string Name2TabId(string name) { return "-" + name.ToLower().Replace(" ", "-").Replace(".", "-"); }

  private ITag Tab(string prefix, string label, bool active = false) {
    return Tag.Li().Class("nav-item").Attr("role", "presentation").Wrap(
      TabButton(prefix, label, Name2TabId(label), active)
    );
  }

  private ITag TabButton(string prefix, string title, string name, bool selected) {
    var id = selected ? "-default" : name;
    return Tag.Button(title).Class("nav-link " + (selected ? "active" : "")).Id(prefix + "-tab")
      .Attr("data-bs-toggle", "tab")
      .Attr("data-bs-target", "#" + prefix + id)
      .Type("button")
      .Attr("role", "tab")
      .Attr("aria-controls", prefix + id)
      .Attr("aria-selected", selected.ToString().ToLower());
  }

  public Div TabContentGroup() {
    return Tag.Div().Class("tab-content p-3 border border-top-0 bg-light mb-5");
  }

  public dynamic TabContentGroupOpen() {
    _tabContentGroupIsOpen = true;
    return TabContentGroup().TagStart;
  }
  private bool _tabContentGroupIsOpen = false;

  public string TabContentGroupClose() {
    var result = _tabContentGroupIsOpen ? "</div>": null;
    _tabContentGroupIsOpen = false;
    return result;
  }

  private Div TabContentDiv(string prefix, string id, bool isActive = false) {
    id = isActive ? "-default" : id;
    return Tag.Div()
        .Class("tab-pane fade " + (isActive ? "show active" : ""))
        .Id(prefix + id)
        .Attr("role", "tabpanel")
        .Attr("aria-labelledby", prefix + id + "-tab");
  }

  public dynamic TabContentOpen(string prefix, string id, bool isActive = false) {
    _tabContentIsOpen = true;
    return TabContentDiv(prefix, id, isActive).TagStart;
  }
  private bool _tabContentIsOpen = false;
  public string TabContentClose() {
    var result = _tabContentIsOpen ? "</div>": null;
    _tabContentIsOpen = false;
    return result;
  }

  public ITag TabContent(string prefix, string id, object result, bool isActive = false) {
    return Tag.RawHtml(
      "\n",
      TabContentDiv(prefix, id, isActive).Wrap(result),
      "\n"
    );
  }

  private string[] _moreTabNames;
  public string GetTabName(int index) {
    var l = Log.Call<string>("index:" + index);
    if (_moreTabNames == null || !_moreTabNames.Any()) return l("no names", "unknown");
    if (_moreTabNames.Length < index + 1) return l("index to high", "unknown");
    var name = _moreTabNames[index];
    Log.Add("name before optimization: '" + name + "'");
    // return l(name, name);
    return name;
  }
}