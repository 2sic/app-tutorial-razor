using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class SourceCodeFormulas: Custom.Hybrid.CodeTyped
{
  #region Init / Dependencies

  public SourceCodeFormulas Init(dynamic sourceCode) {
    SourceCode = sourceCode;
    return this;
  }

  private dynamic SourceCode { get; set; }

  #endregion

  #region Predefined Samples

  public ITypedItem Specs(string sampleId) {
    var list = AsItems(App.Data["TutFormulaSample"]);

    var found = list.FirstOrDefault(s => string.Equals(s.String("TutorialId"), sampleId, StringComparison.InvariantCultureIgnoreCase));
    if (found == null)
      throw new Exception("Sample " + sampleId + " not found");
    return found;
  }

  public bool ShowSnippet(ITypedItem item) {
    return item == null ? false : item.Bool("ShowSnippet");
  }

  #endregion

  public ITag Show(ITypedItem item, bool showIntro = true) {
    return Tag.RawHtml(
      showIntro ? Intro(item) : null,
      ShowFormulas(item)
    );
  }

  public ITag Header(ITypedItem item) {
    if (item.IsEmpty("Title")) return null;
    var title = Tag.H3(item.String("Title"))
      .Attr(Kit.Toolbar.Empty(item).Edit());
    return title;
  }

  public ITag Intro(ITypedItem item) {
    var wrapper = Tag.RawHtml(Tag.H3(
      item.IsNotEmpty("TitleInResults")
        ? item.String("TitleInResults") + " " 
        : "Try it: "),
      DemoToolbar(item, null, item.String("Parameters")).AsTag()
    );
    
    if (item.IsNotEmpty("Instructions"))
      wrapper = wrapper.Add(item.String("Instructions"), Tag.Br());

    wrapper = wrapper.Add(Tag.Em("Click on the (Σ) button above to see the edit-UI with the formula. "));
    return wrapper;
  }

  private ITag ShowFormulas(ITypedItem item) {
    var fields = item.String("Field");

    if (!Text.Has(fields)) return Tag.Comment("No field specified");

    var mainWrapper = Tag.Div();
    foreach (var field in fields.Split(',')) {
      if (!Text.Has(field)) continue;

      var wrapper = Tag.Div().Class("mb-5").Wrap(
        Tag.H3("Formulas of ", Tag.Code(item.String("ContentType") + "." + field))
      );
      var formulas = GetFormulas(item, field);
      foreach (var formula in formulas) {
        wrapper.Add(
          Tag.P(Tag.Strong(formula.Title), " (Formula-Target: " + formula.String("Target") + ")"),
          SourceCode.ShowResultJs(formula.String("Formula"))
        );
      }
      mainWrapper.Add(wrapper);
    }

    return mainWrapper;
  }




  private IEnumerable<ITypedItem> GetFormulas(ITypedItem item, string field) {
    var contentItemType = (App as ToSic.Sxc.Apps.IApp).AppState.GetContentType(item.String("ContentType"));
    var fieldType = contentItemType.Attributes
      .Where(a => a.Name == field)
      .FirstOrDefault();
    
    var attributeMd = AsItems(fieldType.Metadata.OfType("@All")).FirstOrDefault();
    return attributeMd.Children("Formulas");
  }

  // taken from https://fonts.google.com/icons?icon.query=func
  const string iconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"48\" width=\"48\"><path d=\"M12 40v-3.15L25.75 24 12 11.15V8h24v5H19.8l11.75 11L19.8 35H36v5Z\"/></svg>";

  public string IconSvg() { return iconSvg; }
  
  // also used in the 100 basics formula tests
  public dynamic DemoToolbar(ITypedItem item, string moreUi = null, string moreParams = null) {
    return Kit.Toolbar.Empty().New(item.String("ContentType"), ui: new object[] { new { icon = iconSvg }, moreUi }, parameters: item.String("Parameters"));
  }

}