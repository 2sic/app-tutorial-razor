using System.Linq;
using System.Collections.Generic;
using ToSic.Razor.Blade;
public class Helpers : Custom.Hybrid.Code14 {
  public dynamic GetFormulas(string contentType, string fieldName) {
    var contentItemType = App.AppState.GetContentType(contentType);
    var fieldType = contentItemType.Attributes
      .Where(a => a.Name == fieldName)
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

  // public object 

  public dynamic DemoToolbar(string contentType, string more = null) {
    return Kit.Toolbar.Empty().New(contentType, ui: new object[] { UiIcon, more });
  }
}