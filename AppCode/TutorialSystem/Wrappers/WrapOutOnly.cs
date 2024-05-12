using System.Collections.Generic;
using AppCode.TutorialSystem.Sections;
using AppCode.TutorialSystem.Tabs;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;


namespace AppCode.TutorialSystem.Wrappers
{
  internal class WrapOutOnly: Wrap
    {
      public WrapOutOnly(TutorialSectionEngine section) : base(section, "WrapOutOnly",
        tabSpecs: new List<TabSpecs> { new TabSpecs(TabType.Results, Constants.ResultTabName) }
      )
      { }


      public override ITag OutputOpen() { return Tag.RawHtml(
        base.OutputOpen(),
        // "\n",
        // Comment(""),
        TagCount.Open(Tag.Div().Data("start", Name).Class("alert alert-info")),
        Tag.H4(Constants.ResultTitle)
      ); }

      public override ITag OutputClose() { return Tag.RawHtml(base.OutputClose(), TagCount.CloseDiv()); }
    }
}