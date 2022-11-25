using Custom.Hybrid;
using ToSic.Razor.Blade;
using System;
using System.Web;
#if NETCOREAPP
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Text.Encodings.Web;
#endif

// Class to generate shared parts on the page
// Such as navigations etc.
// Should itself not have much code, it's more central API to access everyhing
public class PageParts: Custom.Hybrid.Code14
{
  #region Init / Dependencies
  
  public PageParts Init(dynamic sys) {
    Sys = sys;
    ParentRazor = Sys.ParentRazor;
    return this;
  }
  public dynamic Sys = null;
  public Razor14 ParentRazor;

  #endregion

  // Dnn / Oqtane implementations of render CSHTML based on the given Razor14 base class
  public dynamic RenderPartial(string path, object data = null) {
    var l = Log.Call<dynamic>();
    try {
      #if NETCOREAPP
        var html = ((ParentRazor as dynamic).Html as IHtmlHelper);
        var view = (data == null ? html.Partial(path) : html.Partial(path, data));
        var writer = new StringWriter();
        var encoder = HtmlEncoder.Create(new TextEncoderSettings());
        view.WriteTo(writer, encoder);
        return l("ok oqtane", writer.ToString());
      #else
        var result = (data == null ? ParentRazor.Html.Partial(path) : ParentRazor.Html.Partial(path, data));
        return l("ok dnn", result);
      #endif
    }
    catch
    {
      return l("error", "");
    }
  }

  public dynamic Header(dynamic header = null) {
    var l = Log.Call<dynamic>();
    var headerPart = header != null ? RenderPartial("../shared/_Header.cshtml", header) : null;
    return l(null, Tag.RawHtml(
      headerPart,
      NavPrevNext()
    ));
  }

  public dynamic InfoWrapper() { return Tag.Attr("class", "row mb-5");}

  public dynamic InfoIntro() { return Tag.Attr("class", "col-lg-7");}
  
  public dynamic InfoBoxWrapper() { return Tag.Attr("class", "col-lg-5 order-1");}

  public dynamic FooterWithSource() {
    var l = Log.Call<dynamic>();
    return l("test", Tag.RawHtml(
      NavPrevNext(),
      Sys.SourceCode.ShowCurrentRazor()
    ));
  }

  public dynamic NavPrevNext() {
    return _prevNext ?? (_prevNext = RenderPartial("../shared/_NavigationToolbar.cshtml"));
  }
  private dynamic _prevNext;

}