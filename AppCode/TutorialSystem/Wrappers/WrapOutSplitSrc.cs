using System.Collections.Generic;
using AppCode.TutorialSystem.Sections;
using AppCode.TutorialSystem.Tabs;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using static AppCode.TutorialSystem.Constants;


namespace AppCode.TutorialSystem.Wrappers
{
  internal class WrapOutSplitSrc: Wrap
    {
      public WrapOutSplitSrc(TutorialSectionEngine section) : base(section, "WrapOutSplitSrc",
        tabSpecs: new List<TabSpecs> { new TabSpecs(TabType.ResultsAndSource, ResultAndSourceTabName) }
      )
      {
        FirstWidth = section.Item.Int("OutputWidth", fallback: 50);
      }
      private int FirstWidth;

      public override ITag OutputOpen() { return Tag.RawHtml(
        base.OutputOpen(),
        Comment("", "Splitter"),
        Indent1,
        TagCount.Open(Tag.Div().Id(Section.TabPrefix + "-splitter").Class("splitter-group")),
        "\n" + Indent2 + "<!-- split left -->",
        "\n" + Indent2,
        TagCount.Open(Tag.Div().Id(Section.TabPrefix + "-splitter-left")),
        "\n" + Indent2,
        Tag.H4("⬇️ " + ResultTitle + " | Source ➡️"),
        "\n" + Indent2,
        TagCount.Open(Tag.Div().Class("alert alert-info").Style("margin-right: 10px;")),
        Indent1
      ); }

      public override ITag OutputClose() { return Tag.RawHtml(
        base.OutputClose(),
        "\n" + Indent2,
        TagCount.CloseDiv(),
        "\n" + Indent2 + "<!-- /split-left -->"
        + "\n" + Indent2,
        TagCount.CloseDiv(),
        "\n" + Indent2 + "<!-- split-right -->"
        + "\n" + Indent2,
        TagCount.Open(Tag.Div().Id(Section.TabPrefix + "-splitter-right"))
      ); }

      public override ITag SourceClose() {
        // Ensure it's registered in turnOn
        Section.Kit.Page.TurnOn("window.splitter.init()", data: new {
          parts = new [] {
            "#" + Section.TabPrefix + "-splitter-left",
            "#" + Section.TabPrefix + "-splitter-right"
          },
          options = new {
            sizes = new [] { FirstWidth, 100 - FirstWidth },
          }
        });

        // Return the closing tags
        return Tag.RawHtml(
          Indent2,
          TagCount.CloseDiv(),
          Indent2 + "<!-- /split-right -->\n"
          + "\n" + Indent1,
          TagCount.CloseDiv(),
          Indent1 + "<!-- /Splitter -->\n"
        );
      }
    }
}