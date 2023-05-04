using Custom.Hybrid;
using ToSic.Razor.Blade;
using System.Collections.Generic;
using System.Linq;

public class HeaderHelpers: Custom.Hybrid.Code14
{
  public dynamic AddFeatureLogo(string path, string link, int size = 0) {
    var img = Tag.Div(Tag.Img().Src(App.Path + "/" + path + "?w=75&h=75").Class("img-fluid")).Class("icon-wrapper");
    if (size != 0) img.Style("height: " + size + "px;" + " width: " + size + "px;");
    return Tag.A().Href(link).Target("_blank").Wrap(img.Class("float-right ml-3 ms-3 float-end"));
  }
}