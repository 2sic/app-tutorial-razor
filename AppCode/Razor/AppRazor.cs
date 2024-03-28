using AppCode.Data;
using AppCode.Tutorial;
using AppCode.Output;

namespace AppCode.Razor
{
  partial class AppRazor<TModel>
  {
    public FancyboxService Fancybox => _fancybox ??= GetService<FancyboxService>();
    private FancyboxService _fancybox;

    protected TutLinks TutLinks => _tutLinks ??= GetService<TutLinks>();
    private TutLinks _tutLinks;

    public Sys Sys => _sys ??= GetService<Sys>();
    private Sys _sys;

    public void SetOpenGraph(TutorialGroup tutPage)
    {
      // meta variables
      var metaTitle = tutPage.String("Title", scrubHtml: true);
      
      var metaDescription = tutPage.String("LinkTeaser", scrubHtml: true);
      var hasImg = tutPage.IsNotEmpty("ShareImage");
      var metaImageUrl = hasImg ? tutPage.ShareImage : null;

      // Add open graph meta information
      Kit.Page.AddOpenGraph("og:type", "article");
      Kit.Page.AddOpenGraph("og:title", metaTitle);
      Kit.Page.AddOpenGraph("og:site_name", "Razor Tutorials");
      Kit.Page.AddOpenGraph("og:url", Link.To(parameters: MyPage.Parameters));
      Kit.Page.AddOpenGraph("og:description", metaDescription);
      if (hasImg) {
        Kit.Page.AddOpenGraph("og:image", metaImageUrl);
        Kit.Page.AddOpenGraph("og:image:height", "630");
        Kit.Page.AddOpenGraph("og:image:width", "1200");

        Kit.Page.AddMeta("twitter:image", metaImageUrl);
      }

      // Add twitter meta information
      Kit.Page.AddMeta("twitter:card", "summary_large_image");
      Kit.Page.AddMeta("twitter:title", metaTitle);
      Kit.Page.AddMeta("twitter:description", metaDescription);
    }
  }
}