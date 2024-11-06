using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using AppCode.Data;

namespace AppCode.Tutorial
{

  public class TutLinks: Custom.Hybrid.CodeTyped
  {

    public IHtmlTag TutPageLink(TutorialGroup tutPage) {
      var label = tutPage.String(
        tutPage.IsNotEmpty(nameof(TutorialGroup.LinkTitle)) ? nameof(TutorialGroup.LinkTitle) : nameof(TutorialGroup.Title),
        scrubHtml: "p"
      ) + " ";
      var result = Tag.Li()
        .Attr(Kit.Toolbar.Empty().Edit(tutPage))
        .Wrap(
          Tag.Strong(
            Tag.A(label).Href(TutPageUrl(tutPage)),
            Highlighted(tutPage.LinkEmphasis)
          )
        );
      if (tutPage.IsNotEmpty(nameof(TutorialGroup.LinkTeaser))) {
        result = result.Add(Tag.Br(), tutPage.LinkTeaser);
      } else if (tutPage.IsNotEmpty(nameof(TutorialGroup.Intro))) {
        result = result.Add(Tag.Br(), Text.Ellipsis(tutPage.String(nameof(TutorialGroup.Intro), scrubHtml: true), 250));
      }
      return result;
    }

    public string TutPageUrl(TutorialGroup tutPage) {
      if (tutPage == null) return null;
      // Special / history, the IDs of pages had "-page" and must have it
      // in the URL it's not visible though
      return Link.To(parameters: MyPage.Parameters.Set("tut", tutPage.NameId.BeforeLast("-page")));
    }


    private IHtmlTag Highlighted(string specialText) {
      if (specialText == null) return null;
      return Tag.Span(specialText).Class("text-warning");
    }

  }
}