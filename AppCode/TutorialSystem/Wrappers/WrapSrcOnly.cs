using System.Collections.Generic;
using AppCode.TutorialSystem.Sections;
using AppCode.TutorialSystem.Tabs;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;

namespace AppCode.TutorialSystem.Wrappers
{
  /// <summary>
  /// Show Source only - output is expected to result in empty HTML
  /// </summary>
  internal class WrapSrcOnly: Wrap
    {
      public WrapSrcOnly(TutorialSectionEngine section) : base(section, "WrapSrcOnly",
        tabSpecs: new List<TabSpecs> { TabSpecsFactory.Source() }
      )
      { }

    public override ITag SourceOpen() => Tag.RawHtml(
      "\n",
      Comment("")
    );

    public override ITag SourceClose() => Tag.RawHtml(Comment("/"));
  }
}