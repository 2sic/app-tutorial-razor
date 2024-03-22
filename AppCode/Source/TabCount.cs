using ToSic.Razor.Blade;

namespace AppCode.Source
{
  /// <summary>
  /// Helper class to count tags and add comments
  /// Duplicated in Accordion.cs and SourceCode.cs
  /// Try to keep in sync
  /// </summary>
  public class TagCount {
    public TagCount(string name, bool enabled) { Name = name; Enabled = enabled; }
    public string Name; public bool Enabled; public int Count = 0;
    public string Open() { return "\n<!-- opened " + Name + " OpenCount: " + ++Count + " -->\n"; }
    public string Close() { return "<!-- closed " + Name + " OpenCount: " + --Count + " -->\n"; }

    public IHtmlTag Open(IHtmlTag tag) { return Tag.RawHtml(tag.TagStart, Open()); }
    public IHtmlTag Close(string html) { return Tag.RawHtml(html, Close()); }
    public IHtmlTag CloseDiv() { return Close("</div>"); }
  }
}