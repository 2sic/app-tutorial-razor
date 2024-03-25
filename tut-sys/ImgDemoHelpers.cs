using System;
using System.Collections.Generic;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
public class ImgDemoHelpers: Custom.Hybrid.CodeTyped
{
  public List<string> ShowSrcs = new List<string>();

  public ITag ShowCurrentSrc(string id = null) {
    ShowSrcs.Add(id);
    // Kit.Page.TurnOn("window.showChangingSrc()", data: id);
    Kit.Page.TurnOn("window.imgDemo.start()", data: "img-demo-current-src", noDuplicates: true);
    return Tag.Div().Class("alert alert-light").Wrap(
      Tag.Div("To see the currentSrc change, make the window narrow, reload, and then drag it to become larger."),
      Tag.Code("image src should appear here").Class("img-demo-current-src").Id(id == null ? null : id + "-label")
    );
  }
}
