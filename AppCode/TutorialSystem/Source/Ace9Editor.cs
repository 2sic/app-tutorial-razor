using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;

namespace AppCode.TutorialSystem.Source
{
  public class Ace9Editor: Custom.Hybrid.CodeTyped
  {

    internal Div SourceBlock(ShowSourceSpecs specs, string title) {
      return Tag.Div().Class("code-block " + (specs.Expand ? "is-expanded" : "")).Attr(specs.DomAttribute).Wrap(
        specs.ShowTitle
            ? Tag.H3(title) as ITag
            : Tag.Span(),
        "\n<!-- Raw Source in Pre -->\n",
        SourceBlockCode(specs),
        "\n<!-- /Raw Source in Pre -->\n"
      );
    }



    internal ITag SourceBlockCode(ShowSourceSpecs specs) {
      return Tag.Div().Class("source-code").Wrap(
        "\n",
        Tag.Pre(Tags.Encode(specs.Processed)).Id(specs.RandomId).Style("height: " + specs.Size + "px; font-size: 16px"),
        "\n"
      );
    }

    internal void TurnOnSource(ShowSourceSpecs specs, string filePath, bool wrap) {
      var l = Log.Call("filePath:" + filePath + ", wrap:" + wrap + "; specs.Language: " + specs.Language);
      var language = "ace/mode/" + (specs.Language ?? (Text.Has(filePath)
        ? FindAce9LanguageName(filePath)
        : "html"));

      Kit.Page.TurnOn("window.razorTutorial.initSourceCode()",
        require: "window.ace",
        data: new {
          test = "now-automated",
          domAttribute = specs.DomAttribute,
          aceOptions = new {
            wrap,
            language,
            sourceCodeId = specs.RandomId
          }
        }
      );
      l("language=" + language);
    }

    /// <summary>
    /// Determine the ace9 language of the file
    /// </summary>
    private string FindAce9LanguageName(string filePath) {
      var extension = filePath.Substring(filePath.LastIndexOf('.') + 1);
      switch (extension)
      {
        case "cs": return "csharp";
        case "js": return "javascript";
        case "json": return "json";
        default: return "razor";
      }
    }


  }


}