using System.Linq;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ToSic.Sxc.Images;

namespace AppCode.Output
{

  public class FancyboxService: Custom.Hybrid.CodeTyped
  {
    // Create an image which opens a larger version in a lightbox
    public IHtmlTag PreviewWithLightbox(string url, int width = 100, int height = 100, string classes = "", string label = null, bool figure = true)
    {
      var imgResizeSettings = Kit.Image.Settings(width: width, height: height);
      var html = ForLightbox(
        Kit.Image.Img(url, settings: imgResizeSettings),
        url,
        // Kit.HtmlTags.Img().Src(Link.Image(url, width: width, height: height)),
        classes: classes, label: label
      );

      // If figure is true, wrap the link in a figure tag
      return figure ? Kit.HtmlTags.Figure(html) as IHtmlTag : html;
    }

    public IHtmlTag ForLightbox(IResponsiveImage imgOrPic, string url, string group = null, string classes = "", string label = null)
    {
      // Make sure the fancybox is added to the page (will only add once)
      Kit.Page.Activate("fancybox4"); 

      // Create the link tag with the image inside
      var linkTag = Kit.HtmlTags.A().Data("fancybox", group ?? "gallery").Href(url).Class(classes).Data("caption", label).Wrap(
        imgOrPic
      );

      return linkTag;
    }

    public IHtmlTag Gallery(ITypedItem item, string fieldName, string group = null) {
      if (item == null || !item.ContainsKey(fieldName)) return null;

      var folder = item.Folder(fieldName);
      if (!folder.Files.Any()) return null;

      var t = Kit.HtmlTags;
      var gallery = t.Div();

      var imgList = folder.Files
        .Select(f => ForLightbox(Kit.Image.Picture(f, width: 200), f.Url, group: group, label: f.Metadata.Title))
        .ToArray();
      gallery = gallery.Add(imgList);
      return gallery;
    }
  }
}