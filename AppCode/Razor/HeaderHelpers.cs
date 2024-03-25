using ToSic.Sxc.Apps;
using ToSic.Razor.Blade;

namespace AppCode.Razor
{

  public class HeaderHelpers
  {
    public IHtmlTag AddMainLogo(IAppTyped app, string path, string link, int size = 0) {
      var img = Tag.Div(
        Tag.Img().Src(app.Folder.Url + "/" + path + "?w=75&h=75").Class("img-fluid")
      ).Class("icon-wrapper");

      if (size != 0)
        img.Style("height: " + size + "px;" + " width: " + size + "px;");
      return Tag.A().Href(link).Target("_blank").Wrap(img.Class("float-right ml-3 ms-3 float-end"));
    }

    public IHtmlTag AddFeatureIconFromSegment(string path, string link, int size = 0) {
      var img = Tag.Div(
          Tag.Img().Src(path + "?w=75&h=75")
            .Class("img-fluid")
        )
        .Class("icon-wrapper")
        .Class("float-right ml-3 ms-3 float-end");
      if (size != 0)
        img = img.Style("height: " + size + "px;" + " width: " + size + "px;");
      return link == null
        ? img as IHtmlTag
        : Tag.A().Href(link).Target("_blank").Wrap(img);
    }
  }
}