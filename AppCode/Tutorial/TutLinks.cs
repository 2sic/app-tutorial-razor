using ToSic.Razor.Blade;
using ToSic.Sxc.Data;

namespace AppCode.Tutorial
{

  public class TutLinks: Custom.Hybrid.CodeTyped
  {

    public IHtmlTag TutPageLink(ITypedItem tutPage) {
      var label = tutPage.String(tutPage.IsNotEmpty("LinkTitle") ? "LinkTitle" : "Title", scrubHtml: "p") + " ";
      var result = Tag.Li()
        .Attr(Kit.Toolbar.Empty().Edit(tutPage))
        .Wrap(
          Tag.Strong(
            Tag.A(label).Href(TutPageUrl(tutPage)),
            Highlighted(tutPage.String("LinkEmphasis"))
          )
        );
      if (tutPage.IsNotEmpty("LinkTeaser")) {
        result = result.Add(Tag.Br(), tutPage.String("LinkTeaser"));
      } else if (tutPage.IsNotEmpty("Intro")) {
        result = result.Add(Tag.Br(), Text.Ellipsis(tutPage.String("Intro", scrubHtml: true), 250));
      }
      return result;
    }

    public string TutPageUrl(ITypedItem tutPage) {
      if (tutPage == null) return null;
      return Link.To(parameters: MyPage.Parameters.Set("tut", tutPage.String("NameId").BeforeLast("-Page")));
    }


    private IHtmlTag Highlighted(string specialText) {
      if (specialText == null) { return null; }
      return Tag.Span(specialText).Class("text-warning");
    }

  }
}