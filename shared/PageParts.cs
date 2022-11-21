using Custom.Hybrid;
using ToSic.Razor.Blade;
using System;
using System.Web;

// Class to generate shared parts on the page
// Such as navigations etc.
// Should itself not have much code, it's more central API to access everyhing
public class PageParts: Custom.Hybrid.Code14
{

  public PageParts Init(dynamic sys) {
    Sys = sys;
    ParentRazor = Sys.ParentRazor;
    return this;
  }
  public dynamic Sys = null;
  public Razor14 ParentRazor;

  public dynamic Header(dynamic header = null) {
    var l = Log.Call<dynamic>();
    var headerPart = header != null ? ParentRazor.Html.Partial("../shared/_Header.cshtml", header) : null;
    return l(null, Tag.RawHtml(
      headerPart,
      NavPrevNext()
    ));
  }

  public dynamic FooterWithSource() {
    var l = Log.Call<dynamic>();
    return l("test", Tag.RawHtml(
      NavPrevNext(),
      Sys.SourceCode.ShowCurrentRazor()
    ));
  }

  public dynamic NavPrevNext() {
    return _prevNext ?? (_prevNext = ParentRazor.Html.Partial("../shared/_NavigationToolbar.cshtml"));
  }
  private dynamic _prevNext;

}