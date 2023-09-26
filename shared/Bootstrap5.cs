using ToSic.Razor.Blade;

public class Bootstrap5 : Custom.Hybrid.Code14
{
  // if the theme framework is not BS4, just activate/load it from the WebResources
  // this solves both the cases where its unknown, or another framework
  public void EnsureBootstrap5()
  {
    if (Kit.Css.IsUnknown) {
      Kit.Page.Activate("Bootstrap5");
    }
  }

  // show warning for admin if koi.json is missing
  public IHtmlTag WarnAboutMissingOrUnknownBootstrap() {
    if (Kit.Css.IsUnknown && CmsContext.User.IsSiteAdmin) {
      return Tag.Div().Class("alert alert-warning").Wrap(
        Connect.Koi.Messages.CssInformationMissing,
        Tag.Br(),
        Connect.Koi.Messages.OnlyAdminsSeeThis
      );
    }
    return null;
  }
}
