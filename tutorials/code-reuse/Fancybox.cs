using ToSic.Razor.Blade;

public class Fancybox: Custom.Hybrid.CodeTyped
{
  // Create an image which opens a larger version in a lightbox
  public IHtmlTag PreviewWithLightbox(string url, int width = 100, int height = 100, string classes = "", string label = null)
  {
    // Add fancybox JS to the page (will only add once, no matter how often it's called)
    Kit.Page.Activate("fancybox4"); 

    var imgResizeSettings = Kit.Image.Settings(width: width, height: height);
    var imgOrPic = Kit.Image.Img(url, settings: imgResizeSettings);
    // Create the link tag with the image inside
    var html = Kit.HtmlTags.A()
      .Data("fancybox", "gallery")
      .Href(url)
      .Class(classes)
      .Data("caption", label)
      .Wrap(imgOrPic);

    return html;
  }
}