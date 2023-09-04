using ToSic.Razor.Blade;

public class Accordion: Custom.Hybrid.CodeTyped
{
  public IHtmlTag Start(string name) {
    Name = name;
    return Tag.RawHtml(
      "<!-- Start(" + name + ") -->" 
      + "\n"
      + Kit.HtmlTags.Div().Class("accordion").Id(name).TagStart
    );
  }

  public string Name { get; private set; }

  public IHtmlTag End() { return Kit.HtmlTags.RawHtml(DivEnd); }

  public AccPart Section(string title) {
    return new AccPart(this, Kit.HtmlTags, Name + "-" + AutoPartName + AutoPartIndex++, title);
  }
  private const string AutoPartName = "auto-part-";
  private int AutoPartIndex = 0;

  internal string DivEnd = "</div>";
}

public class AccPart {
  public AccPart(Accordion accordion, IHtmlTagsService tags, string name, string title = null) {
    Acc = accordion;
    Name = name;
    Title = title;
    TagsSvc = tags;
  }
  private Accordion Acc;
  public string Name { get; private set; }
  public string Title { get; private set; }

  private IHtmlTagsService TagsSvc;
  public string HeadingId { get { return Name + "-heading"; } }
  public string BodyId { get { return Name + "-body"; } }

  private string Indent = "    ";

  public IHtmlTag Start() { 
    return TagsSvc.RawHtml(
      "\n",
      Indent,
      "<!-- Part.Start(" + Name + ") -->",
      "\n",
      Indent,
      TagsSvc.Div().Class("accordion-item").TagStart,
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
        .Wrap(Title),
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
      "\n"
    );
  }
  #endregion

}
