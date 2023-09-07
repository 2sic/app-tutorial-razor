using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;

public class Accordion: Custom.Hybrid.CodeTyped
{
  public Accordion Setup(object sys, string variantExtension) {
    Sys = sys;
    _variantExtension = variantExtension;
    return this;
  }

  private dynamic Sys;
  private string _variantExtension;

  public IHtmlTag Start(ITypedItem item) {
    Item = item;
    Name = item.String("TutorialId");
    return StartInner();
  }

  private IHtmlTag StartInner() {
    var t = Kit.HtmlTags;
    var heading = t.H2(Item.String("Title", scrubHtml: "p")).Class("quick-ref");
    heading = (Item.Id != 0)
      ? heading.Attr(Kit.Toolbar.Empty(Item).Edit())
      : heading.Attr(Kit.Toolbar.Empty().New("TutAccordion", prefill: new { TutorialId = Item.String("TutorialId") }));
    return t.RawHtml(
      "\n<!-- Accordion.Start(" + Name + ") -->\n",
      heading,
      "\n",
      Item.Html("Intro"),
      "\n",
      t.Div().Class("accordion").Id(Name).TagStart
    );
  }

  public string Name { get; private set; }

  public ITypedItem Item { get; private set; }

  public IHtmlTag End() {
    var end = Kit.HtmlTags.RawHtml(DivEnd);
    Item = null;
    return end;
  }

  private string NextName() { return Name + "-" + AutoPartName + AutoPartIndex++; }

  public IEnumerable<Section> Sections(string basePath, string pathPrefix) {
    if (Item == null) throw new Exception("Item in Accordion is null");
    basePath = Text.BeforeLast(basePath, "/");
    var names = Item.Children("Sections")
      .Select(itm => {
        string fileName;
        if (!CheckFile(basePath, pathPrefix, itm.String("TutorialId"), null, out fileName))
          CheckFile(basePath, pathPrefix, itm.String("TutorialId"), _variantExtension, out fileName);
        return new Section(this, Kit.HtmlTags, NextName(), item: itm, fileName: fileName);
      })
      .ToList();
    return names;
  }

  private bool CheckFile(string basePath, string pathPrefix, string tutorialId, string suffix, out string fileName) {
    fileName = pathPrefix + tutorialId + suffix + ".cshtml";
    var filePath = System.IO.Path.Combine(basePath, fileName);
    var fullPath = Sys.SourceCode.GetFullPath(filePath);
    if (System.IO.File.Exists(fullPath)) return true;
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
  }
  private Accordion Acc;
  public string Name { get; private set; }
  public ITypedItem Item { get; private set; }

  private IHtmlTagsService TagsSvc;
  public string HeadingId { get { return Name + "-heading"; } }
  public string BodyId { get { return Name + "-body"; } }
  public string TutorialId { get { return Item.String("TutorialId"); } }

  private string Indent = "    ";

  public IHtmlTag Start() { 
    var start = TagsSvc.Div().Class("accordion-item");
    if (Item != null)
      start = (Item.Id != 0)
        ? start.Attr(Acc.Kit.Toolbar.Empty(Item).Edit())
        : start.Attr(Acc.Kit.Toolbar.Empty().New("TutAccordionSection", prefill: new { TutorialId }));
    return TagsSvc.RawHtml(
      "\n",
      Indent,
      "<!-- Part.Start(" + Name + ") -->",
      "\n",
      Indent,
      start.TagStart,
      "\n",
      Indent,
      Header(),
      "\n",
      BodyStart()
    );
  }

  public string SectionFile { get; private set; }

  public IHtmlTag End() {
    return TagsSvc.RawHtml("\n", Acc.DivEnd, "\n", Acc.DivEnd, "\n", Acc.DivEnd, "\n");
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
        .Wrap(Item.String("Title", scrubHtml: "p")),
      "\n",
      Indent
    );
  }

  private IHtmlTag BodyStart() {
    return TagsSvc.RawHtml(
      "\n",
      Indent,
      TagsSvc.Div().Id(BodyId).Class("accordion-collapse collapse").Attr("aria-labelledby", HeadingId).Data("bs-parent", "#" + Acc.Name).TagStart,
      "\n",
      Indent2,
      TagsSvc.Div().Class("accordion-body").TagStart,
      "\n",
      (Item == null ? "" : Indent2 + Item.Html("Intro"))
    );
  }
  #endregion

}
