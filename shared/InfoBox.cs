using ToSic.Razor.Blade;
using ToSic.Sxc.Services;
public class InfoBox: Custom.Hybrid.Code12
{
  public dynamic InfoSection(dynamic content, string title, string icon = "fa-info-circle") {
    return Tag.Div(Tag.H6(Tag.I().Class("fas " + icon), Tag.Span(title).Class("ml-2")).Class("card-header"), Tag.Div(Tag.Div(Tag.Ul(content).Class("list-unstyled list-group-item ml-2")).Class("list-group list-group-flush"))).Class("card");
  }
  public dynamic InfoContent(string title, string link = "", string textColor = "") {
    var content = Tag.Li(title);
    if (Text.Has(link)) {
      return Tag.A(content).Href(link).Target("_blank").Class(textColor);
    }
    return content.Class(textColor);
  }
}