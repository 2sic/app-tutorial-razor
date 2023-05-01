using ToSic.Razor.Blade;
public class Fancybox: Custom.Hybrid.Code14
{
  // Create an image which opens a larger version in a lightbox
  public IHtmlTag PreviewWithLightbox(string url, int width = 100, int height = 100, string classes = "", string label = null, bool figure = true)
  {
    // Make sure the fancybox is added to the page, but only once
    Kit.Page.Activate("fancybox4"); 

    var linkTag = Tag.A().Attr("data-fancybox='gallery'").Href(url).Class(classes).Attr("data-caption", label).Wrap(
        Tag.Img().Src(Link.Image(url, width: width, height: height))
      );
    return figure ? Tag.Figure(linkTag) as IHtmlTag : linkTag;
  }
}