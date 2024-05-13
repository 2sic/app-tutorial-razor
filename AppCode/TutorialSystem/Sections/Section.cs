using AppCode.Data;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;

namespace AppCode.TutorialSystem.Sections
{
  /// <summary>
  /// Accordion Part (Section)
  /// </summary>
  public class Section
  {
    public Section(Accordion accordion, IHtmlTagsService tags, string name, TutorialSnippet item = null, string fileName = null, VariantMatch variantMatch = VariantMatch.General) {
      Acc = accordion;
      Name = name;
      Item = item;
      TagsSvc = tags;
      CodeFile = fileName;
      Show = Acc.Item.DefaultStateIsOpen; // later we can add more conditions
      VariantMatch = variantMatch;
    }
    private readonly Accordion Acc;
    public readonly string Name;
    public readonly TutorialSnippet Item;

    private readonly IHtmlTagsService TagsSvc;
    public string HeadingId => Name + "-heading";
    public string BodyId => Name + "-body";
    public string TutorialId => Item.TutorialId;

    private bool Show {get;set;}
    private readonly string Indent = "    ";

    private VariantMatch VariantMatch { get; }

    public IHtmlTag Start() { 
      var start = TagsSvc.Div().Class("accordion-item");
      if (Item != null)
        start = (Item.Id != 0)
          ? start.Attr(Acc.Kit.Toolbar.Empty(Item).Edit().New())
          : start.Attr(Acc.Kit.Toolbar.Empty().New("TutorialSnippet", prefill: new { TutorialId }));
      return TagsSvc.RawHtml(
        "\n" + Indent + "<!-- Part.Start(" + Name + ") -->\n",
        Indent,
        Acc.TagCount.Open(start),
        Indent,
        Header(),
        "\n\n",
        Indent +  "<!-- Part.Body(" + Name + ") -->\n",
        BodyStart()
      );
    }

    /// <summary>
    /// The real code file after detecting the variant, eg. /some-path/Snip-identifier.Typed.Cshtml
    /// </summary>
    public string CodeFile { get; private set; }

    public IHtmlTag End() {
      return TagsSvc.RawHtml(
        "\n",
        Acc.TagCount.Close(Acc.DivEnd + "<!-- /Acc 1 ? -->"),
        Acc.TagCount.Close(Acc.DivEnd + "<!-- /Acc 2 ? -->"),
        Acc.TagCount.Close(Acc.DivEnd + "<!-- /Acc 3 ? -->")
      );
    }

    #region Helpers to build Start
    private const string Indent2 = "      ";
    private IHtmlTag Header() {
      var variantIcon = VariantMatch switch {
        VariantMatch.Exact => "🎯",
        VariantMatch.Fallback => "🪂",
        VariantMatch.General => "🪖",
        VariantMatch.NotFound => "❌",
        _ => "❓"
      };
      var variantTitle = VariantMatch switch {
        VariantMatch.Exact => "Exact Match",
        VariantMatch.Fallback => "Fallback Match",
        VariantMatch.General => "General Match",
        VariantMatch.NotFound => "Not Found",
        _ => "Unknown"
      };
      var tutIdPath = new TutorialIdToPath(TutorialId, "").FullPath;
      return TagsSvc.H2().Class("accordion-header").Id(HeadingId).Wrap(
        "\n",
        Indent2,
        TagsSvc.Button()
          .Class("accordion-button " + (Show ? "" : "collapsed"))
          .Type("button")
          .Data("bs-toggle", "collapse")
          .Data("bs-target", "#" + BodyId)
          .Attr("aria-expanded", "false")
          .Attr("aria-controls", BodyId)
          .Wrap(
            Item.String("Title", scrubHtml: "p"),
            Acc.MyUser.IsSystemAdmin
              ? TagsSvc.Span(
                  Text.Ellipsis(tutIdPath.Replace("tutorials/", ""), 40),
                  TagsSvc.Span("ℹ️").Title("This is the snip '" + TutorialId + "' - to be found in " + tutIdPath),
                  TagsSvc.Span(variantIcon).Title(variantTitle)
                ).Style("flex: 1 0 auto; text-align: right; margin-right: 60px;")
              : null
          ),
        "\n",
        Indent
      );
    }

    private IHtmlTag BodyStart() {
      var note = Item != null && Item.IsNotEmpty("Note")
        ? TagsSvc.Div().Class("alert alert-warning").Wrap(Item.Note)
        : null;

      return TagsSvc.RawHtml(
        "\n",
        Indent,
        Acc.TagCount.Open(TagsSvc.Div().Id(BodyId)
          .Class("accordion-collapse collapse " + (Show ? "show" : ""))
          .Attr("aria-labelledby", HeadingId)
          .Data("bs-parent", "#" + Acc.Name)),
        "\n",
        Indent2,
        Acc.TagCount.Open(TagsSvc.Div().Class("accordion-body")),
        "\n",
        Indent2 + Item?.Html("Intro"),
        "\n",
        Indent2 + Item?.Html("IntroMore" + Acc.VariantFieldSuffix),
        "\n",
        note,
        "\n"
      );
    }
    #endregion

  }

}