using ToSic.Razor.Blade;

public class Accordion: Custom.Hybrid.CodePro
{
  public object Start(string name) {
    Name = name;
    return Kit.HtmlTags.Div().Class("accordion").Id(name).TagStart;
  }

  public string Name {get;set;}

  public object End() { return DivEnd; }

  public IHtmlTag ItemStart(string partName, string title) {
    var specs = new AccPart(Name, partName);
    return Kit.HtmlTags.RawHtml(
      Kit.HtmlTags.Div().Class("accordion-item").TagStart,
      "\n",
      Header(specs, title),
      "\n",
      BodyStart(specs)
    );
  }
  public IHtmlTag ItemEnd() {
    return Kit.HtmlTags.RawHtml("\n", DivEnd, "\n", DivEnd, "\n", DivEnd, "\n");
  }


  public IHtmlTag Header(AccPart specs, string title) {
    return Kit.HtmlTags.H2().Class("accordion-header").Id(specs.HeadingId).Wrap(
        Kit.HtmlTags.Button()
          .Class("accordion-button collapsed")
          .Type("button")
          .Data("bs-toggle", "collapse")
          .Data("bs-target", "#" + specs.BodyId)
          .Attr("aria-expanded", "false")
          .Attr("aria-controls", specs.BodyId)
          .Wrap(title)
    );
  }

  #region Body

  public IHtmlTag BodyStart(AccPart specs) {
    return Kit.HtmlTags.RawHtml(
      Kit.HtmlTags.Div().Id(specs.BodyId).Class("accordion-collapse collapse").Attr("aria-labelledby", specs.HeadingId).Data("bs-parent", "#" + specs.Parent).TagStart 
      + "\n"
      + Kit.HtmlTags.Div().Class("accordion-body").TagStart
      + "\n"
    );
  }

  public IHtmlTag BodyEnd() {
    return Kit.HtmlTags.RawHtml(DivEnd, "\n", DivEnd);
  }

  #endregion

  private object DivEnd { get { return _divEnd ?? (_divEnd = Kit.HtmlTags.Div().TagEnd); } }
  private object _divEnd;
}

public class AccPart {
  public AccPart(string parent, string name) {
    Parent = parent;
    Name = name;
  }
  public string Parent { get; set; }
  public string Name { get; set; }
  public string HeadingId { get { return Name + "-heading"; } }
  public string BodyId { get { return Name + "-body"; } }
}
