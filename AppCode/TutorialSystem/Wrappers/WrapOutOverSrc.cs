using AppCode.TutorialSystem.Sections;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;


namespace AppCode.TutorialSystem.Wrappers
{
  /// <summary>
  /// Show Output inside a box above Source
  /// </summary>
  internal class WrapOutOverSrc: Wrap
    {
      private const string nameOfClass = "WrapOutOverSrc";
      public WrapOutOverSrc(TutorialSectionEngine section) : base(section, nameOfClass, tabsCsv: Constants.ResultAndSourceTabName) {
      }


      public override ITag OutputOpen() { return Tag.RawHtml(
        base.OutputOpen(),
        // "\n",
        // Comment(nameOfClass),
        TagCount.Open(Tag.Div().Data("start", Name).Class("alert alert-info")),
        Tag.H4(Constants.ResultTitle)
      ); }

      public override ITag OutputClose() { return Tag.RawHtml(base.OutputClose(), TagCount.CloseDiv()); }
    }
}