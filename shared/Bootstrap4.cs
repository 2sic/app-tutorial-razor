using ToSic.Razor.Blade;
using Connect.Koi;

public class Bootstrap4 : Custom.Hybrid.Code12
{
  public dynamic EnsureBootstrap4()
  {
    var pageCss = GetService<ICss>();
    // if the theme framework is not BS4, just include it
    // this solves both the cases where its unknown, or another framework
    if(pageCss.Is("bs4")) { return null; }
    return Tag.Custom(
      "<link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css' integrity='sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T' crossorigin='anonymous'>"
    );
  }

  public dynamic WarnAboutMissingOrUnknownBootstrap() {
    var pageCss = GetService<ICss>();

    if(!CmsContext.User.IsSiteAdmin) { return null; }
    var message = pageCss.IsUnknown
      ? "This template could not detect if bootstrap4 was already included in the page or not. Because of this, we auto-included it - but this may be unnecessary and slow down this page. Please fix, by supplying a koi.json file in the theme folder."
      : !pageCss.Is("bs4")
        ? "Your theme seems to use a css framework different than bootstrap4, but these templates are optimized for it. Bootstrap4 was auto-included for your convenience. For performance reasons, we suggest you either switch to bootstrap4 or optimize these templates to work with the css-framework you prefer. "
        : "";

    if(message == "") { return null; }
    var msg = Tag.Div().Class("alert alert-warning").Attr("role", "alert")
        .Wrap(
          Tag.Strong("Warning for page admins only:"),
          message,
          Tag.Br(),
          "You can also remove this message by commenting out the code calling <code>WarnAboutMissingOrUnknownBootstrap()</code>"
        );
    return msg;
  }
}
