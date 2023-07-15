using ToSic.Razor.Blade;
public class Fancybox: Custom.Hybrid.CodePro
{
  // Create an image which opens a larger version in a lightbox
  public IHtmlTag PreviewWithLightbox(string url, int width = 100, int height = 100, string classes = "", string label = null, bool figure = true)
  {
    // Make sure the fancybox is added to the page (will only add once)
    Kit.Page.Activate("fancybox4"); 

    // Create the link tag with the image inside
    var linkTag = Kit.HtmlTags.A().Data("fancybox", "gallery").Href(url).Class(classes).Data("caption", label).Wrap(
      Kit.HtmlTags.Img().Src(Link.Image(url, width: width, height: height))
    );

    // If figure is true, wrap the link in a figure tag
    return figure ? Kit.HtmlTags.Figure(linkTag) as IHtmlTag : linkTag;
  }
}