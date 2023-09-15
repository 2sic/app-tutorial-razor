using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;

public class Accordion: Custom.Hybrid.CodeTyped
{
  public Accordion Setup(object sys, string variantExtension, ITypedItem item = null) {
    Sys = sys;
    _variantExtension = variantExtension;
    Item = item;
    return this;
  }
  public TagCount TagCount = new TagCount("Accordion", true);
  private dynamic Sys;
  private string _variantExtension;

  public IHtmlTag Start(ITypedItem item) {
    Item = item;
    Name = item.String("NameId");
    if (!_variantExtension.Has()) {
      var isTyped = (MyPage.Parameters["variant"] ?? "typed") == "typed";
      _variantExtension = isTyped ? ".Typed" : ".Dyn";
    }
    return StartInner();
  }

  public bool IsTyped  { get { return (MyPage.Parameters["variant"] ?? "typed") == "typed"; }}
  public string Variant { get { return IsTyped ? "typed" : "dynamic"; }}

  private IHtmlTag StartInner() {
    var t = Kit.HtmlTags;
    var heading = t.H2().Class("quick-ref").Wrap(
      Item.String("Title", scrubHtml: "p")
    );
    // Add Toolbar
    heading = (Item.Id != 0)
      ? heading.Attr(Kit.Toolbar.Empty(Item).Edit().New())
      : heading.Attr(Kit.Toolbar.Empty().New("TutAccordion", prefill: new { NameId = Name }));

    var note = Item.IsNotEmpty("Note")
      ? t.Div().Class("alert alert-warning").Wrap(Item.String("Note"))
      : null;

    return t.RawHtml(
      "\n<!-- Accordion.Start(" + Name + ") -->\n",
      heading,
      "\n",
      Item.Html("Intro"),
      "\n",
      note,
      "\n",
      TagCount.Open(t.Div().Class("accordion").Id(Name))
    );
  }

  public string Name { get; private set; }

  public ITypedItem Item { get; private set; }

  public IHtmlTag End() {
    var end = TagCount.Close(DivEnd + "<!-- /Accordion -->");
    Item = null;
    return end;
  }

  private string NextName() { return Name + "-" + AutoPartName + AutoPartIndex++; }

  public IEnumerable<Section> Sections(string basePath, string backtrack) {
    if (Item == null) throw new Exception("Item in Accordion is null");
    var appPath = App.Folder.Path;
    basePath = Text.BeforeLast(basePath, "/");
    var names = Item.Children("Sections")
      .Select(itm => {
        var tutorialId = itm.String("TutorialId");
        string fileName;
        if (!CheckFile(appPath, backtrack, tutorialId, null, out fileName))
          CheckFile(appPath, backtrack, tutorialId, _variantExtension, out fileName);
        return new Section(this, Kit.HtmlTags, NextName(), item: itm, fileName: fileName);
      })
      .ToList();
    return names;
  }

  private bool CheckFile(string appPath, string relBacktrack, string tutorialId, string variant, out string fileName) {
    var topPath = Text.Before(tutorialId, "-");
    var rest = Text.After(tutorialId, "-");
    var secondPath = Text.Before(rest, "-");
    rest = Text.After(rest, "-");

    if (!Text.Has(secondPath))
      throw new Exception("Second path is empty, original was '" + tutorialId + "'");

    var realName = "Snip-" + rest + variant + ".cshtml";
    var filePath = System.IO.Path.Combine(appPath, topPath, secondPath, realName);
    var fullPath = Sys.SourceCode.GetFullPath(filePath);
    if (System.IO.File.Exists(fullPath)) {
      fileName = relBacktrack + "/" + System.IO.Path.Combine(topPath, secondPath, realName);
      return true;
    }
    fileName = null;
    return false;
  }

  private const string AutoPartName = "auto-part-";
  private int AutoPartIndex = 0;

  internal string DivEnd = "</div>";
}


/// <summary>
/// Accordion Part (Section)
/// </summary>
public class Section {
  public Section(Accordion accordion, IHtmlTagsService tags, string name, ITypedItem item = null, string fileName = null) {
    Acc = accordion;
    Name = name;
    Item = item;
    TagsSvc = tags;
    SectionFile = fileName;
    Show = Acc.Item.Bool("DefaultStateIsOpen"); // later we can add more conditions
  }
  private Accordion Acc;
  public string Name { get; private set; }
  public ITypedItem Item { get; private set; }

  private IHtmlTagsService TagsSvc;
  public string HeadingId { get { return Name + "-heading"; } }
  public string BodyId { get { return Name + "-body"; } }
  public string TutorialId { get { return Item.String("TutorialId"); } }

  private bool Show {get;set;}
  private string Indent = "    ";

  public IHtmlTag Start() { 
    var start = TagsSvc.Div().Class("accordion-item");
    if (Item != null)
      start = (Item.Id != 0)
        ? start.Attr(Acc.Kit.Toolbar.Empty(Item).Edit().New())
        : start.Attr(Acc.Kit.Toolbar.Empty().New("TutAccordionSection", prefill: new { TutorialId }));
    return TagsSvc.RawHtml(
      "\n" + Indent + "<!-- Part.Start(" + Name + ") -->\n",
      Indent,
      Acc.TagCount.Open(start),
      Indent,
      Header(),
      "\n\n",
      Indent +  "<!-- Part.Body(" + Name + ") -->\n",
      BodyStart()
    );
  }

  public string SectionFile { get; private set; }

  public IHtmlTag End() {
    return TagsSvc.RawHtml(
      "\n",
      Acc.TagCount.Close(Acc.DivEnd + "<!-- /Acc 1 ? -->"),
      Acc.TagCount.Close(Acc.DivEnd + "<!-- /Acc 2 ? -->"),
      Acc.TagCount.Close(Acc.DivEnd + "<!-- /Acc 3 ? -->")
    );
  }

  #region Helpers to build Start
  private const string Indent2 = "      ";
  private IHtmlTag Header() {
    return TagsSvc.H2().Class("accordion-header").Id(HeadingId).Wrap(
      "\n",
      Indent2,
      TagsSvc.Button()
        .Class("accordion-button collapsed")
        .Type("button")
        .Data("bs-toggle", "collapse")
        .Data("bs-target", "#" + BodyId)
        .Attr("aria-expanded", "false")
        .Attr("aria-controls", BodyId)
        .Wrap(
          // TagsSvc.Span("test").Style("float: right"),
          Item.String("Title", scrubHtml: "p"),
          Acc.MyUser.IsSystemAdmin
            ? TagsSvc.Span("ℹ️").Title("This is the snip '" + TutorialId + "'").Style("flex: 1 0 auto; text-align: right; margin-right: 60px;")
            : null
        ),
      "\n",
      Indent
    );
  }

  private IHtmlTag BodyStart() {
    var note = Item != null && Item.IsNotEmpty("Note")
      ? TagsSvc.Div().Class("alert alert-warning").Wrap(Item.String("Note"))
      : null;

    return TagsSvc.RawHtml(
      "\n",
      Indent,
      Acc.TagCount.Open(TagsSvc.Div().Id(BodyId)
        .Class("accordion-collapse collapse " + (Show ? "show" : ""))
        .Attr("aria-labelledby", HeadingId)
        .Data("bs-parent", "#" + Acc.Name)),
      "\n",
      Indent2,
      Acc.TagCount.Open(TagsSvc.Div().Class("accordion-body")),
      "\n",
      (Item == null ? "" : Indent2 + Item.Html("Intro")),
      "\n",
      (Item == null ? "" : Indent2 + Item.Html("IntroMore" + (Acc.IsTyped ? "Typed" : "Dyn"))),
      "\n",
      note,
      "\n"
    );
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
}