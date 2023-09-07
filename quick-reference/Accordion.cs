using System.Collections.Generic;
using System.Linq;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;

public class Accordion: Custom.Hybrid.CodeTyped
{
  public IHtmlTag Start(string name) {
    Name = name;
    Item = GetAccordionData(name);
    var t = Kit.HtmlTags;
    var heading = t.H2(Item.String("Title", scrubHtml: "p")).Class("quick-ref");
    heading = (Item.Id != 0)
      ? heading.Attr(Kit.Toolbar.Empty(Item).Edit())
      : heading.Attr(Kit.Toolbar.Empty().New("TutAccordion", prefill: new { TutorialId = Item.String("TutorialId") }));
    return t.RawHtml(
      "\n<!-- Accordion.Start(" + name + ") -->\n",
      heading,
      t.Div().Class("accordion").Id(name).TagStart
    );
  }

  public string Name { get; private set; }

  public ITypedItem Item { get; private set; }

  public IHtmlTag End() { return Kit.HtmlTags.RawHtml(DivEnd); }

  public Section Section(string tutorialId) {
    return new Section(this, Kit.HtmlTags, Name + "-" + AutoPartName + AutoPartIndex++, item: GetSectionData(tutorialId));
  }

  private const string AutoPartName = "auto-part-";
  private int AutoPartIndex = 0;

  internal string DivEnd = "</div>";

  private ITypedItem GetAccordionData(string tutorialId) {
    if (_accordionData == null) _accordionData = AsItems(App.Data["TutAccordion"]);
    return _accordionData.FirstOrDefault(s => s.String("TutorialId") == tutorialId)
      ?? AsItem(new { Title = "Accordion '" + tutorialId + "' not found", TutorialId = tutorialId }, mock: true, propsRequired: false);
  }
  private IEnumerable<ITypedItem> _accordionData;

  private ITypedItem GetSectionData(string tutorialId) {
    if (_sectionData == null) _sectionData = AsItems(App.Data["TutAccordionSection"]);
    return _sectionData.FirstOrDefault(s => s.String("TutorialId") == tutorialId)
      ?? AsItem(new { Title = "Section '" + tutorialId + "' not found", TutorialId = tutorialId }, mock: true, propsRequired: false);
  }
  private IEnumerable<ITypedItem> _sectionData;
}


/// <summary>
/// Accordion Part (Section)
/// </summary>
public class Section {
  public Section(Accordion accordion, IHtmlTagsService tags, string name, ITypedItem item = null) {
    Acc = accordion;
    Name = name;
    Item = item;
    TagsSvc = tags;
  }
  private Accordion Acc;
  public string Name { get; private set; }
  public ITypedItem Item { get; private set; }

  private IHtmlTagsService TagsSvc;
  public string HeadingId { get { return Name + "-heading"; } }
  public string BodyId { get { return Name + "-body"; } }

  private string Indent = "    ";

  public IHtmlTag Start() { 
    var start = TagsSvc.Div().Class("accordion-item");
    if (Item != null)
      start = (Item.Id != 0)
        ? start.Attr(Acc.Kit.Toolbar.Empty(Item).Edit())
        : start.Attr(Acc.Kit.Toolbar.Empty().New("TutAccordionSection", prefill: new { TutorialId = Item.String("TutorialId") }));
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
