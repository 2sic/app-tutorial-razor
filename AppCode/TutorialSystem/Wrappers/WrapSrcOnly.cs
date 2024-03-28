using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using AppCode.Source;

namespace AppCode.TutorialSystem.Wrappers
{
  /// <summary>
  /// Show Source only - output is expected to result in empty HTML
  /// </summary>
  internal class WrapSrcOnly: Wrap
    {
      public WrapSrcOnly(TutorialSection section) : base(section, "WrapSrcOnly", tabsCsv: Constants.SourceTabName)
      { }

      public override ITag SourceOpen() { return Tag.RawHtml(
        "\n",
        Comment("")
      ); }

      public override ITag SourceClose() { return Tag.RawHtml(Comment("/")); }
    }
}