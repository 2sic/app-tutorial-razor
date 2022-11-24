using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

public class SourceCodeFormulas: Custom.Hybrid.Code14
{
  #region Init / Dependencies

  public SourceCodeFormulas Init(dynamic sourceCode) {
    SourceCode = sourceCode;
    return this;
  }

  private dynamic SourceCode { get; set; }

  #endregion

  #region Predefined Samples

  public class FormulaSpecs {
    public string Title;
    public string Instructions;
    public string ContentType;
    public string Field;
    public string Parameters;

    public bool ShowSnippet = false;
  }

  public FormulaSpecs Specs(string contentType, string field) {
    return Specs(null, null, contentType, field, null);
  }

  public FormulaSpecs Specs(string title, string instructions, string contentType, string field, string parameters = null)
  {
    return new FormulaSpecs() {
      Title = title,
      Instructions = instructions,
      ContentType = contentType,
      Field = field,
      Parameters = parameters
    };
  }

  public bool ShowSnippet(object specsRaw) {
    var specs = specsRaw as FormulaSpecs;
    return specs == null ? false : specs.ShowSnippet;
  }

  #endregion

  public ITag Show(object specsRaw, bool showIntro = true) {
    var specs = specsRaw as FormulaSpecs;
    return Tag.RawHtml(
      showIntro ? Intro(specs) : null,
      ShowFormulas(specs)
    );
  }

  public ITag Intro(object specsRaw) {
    var specs = specsRaw as FormulaSpecs;

    if (!Text.Has(specs.Title)) return null;
    var wrapper = Tag.RawHtml(
      Tag.H3(specs.Title + " ", DemoToolbar(specs, null, specs.Parameters).AsTag())
    );
    if (Text.Has(specs.Instructions)) {
      wrapper.Add(specs.Instructions, Tag.Br());
    }
    wrapper.Add(Tag.Em("Click on the (Σ) button above to see the edit-UI with the formula. "));
    return wrapper;
  }

  private ITag ShowFormulas(FormulaSpecs specs) {

    var wrapper = Tag.Div().Class("mb-5").Wrap(
      Tag.H3("Formulas of ", Tag.Code(specs.ContentType + "." + specs.Field))
    );
    var formulas = GetFormulas(specs);
    foreach (var formula in formulas) {
      wrapper.Add(
        Tag.P(Tag.Strong(formula.Title), "(Formula-Target: " + formula.Target + ")"),
        SourceCode.ShowResultJs(formula.Formula)
      );
    }
    return wrapper;
  }




  private dynamic GetFormulas(FormulaSpecs specs) {
    var contentItemType = App.AppState.GetContentType(specs.ContentType);
    var fieldType = contentItemType.Attributes
      .Where(a => a.Name == specs.Field)
      .FirstOrDefault();
    
    var attributeMd = AsList(fieldType.Metadata.OfType("@All") as object).FirstOrDefault();
    return AsList(attributeMd.Formulas as object);
  }

  // taken from https://fonts.google.com/icons?icon.query=func
  const string iconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"48\" width=\"48\"><path d=\"M12 40v-3.15L25.75 24 12 11.15V8h24v5H19.8l11.75 11L19.8 35H36v5Z\"/></svg>";

  public static object UiIcon = new {
    icon = iconSvg
  };

  public object Icon() {
    return UiIcon;
  }

  
  // also used in the 100 basics formula tests
  public dynamic DemoToolbar(FormulaSpecs specs, string moreUi = null, string moreParams = null) {
    return Kit.Toolbar.Empty().New(specs.ContentType, ui: new object[] { UiIcon, moreUi }, parameters: specs.Parameters);
  }

}