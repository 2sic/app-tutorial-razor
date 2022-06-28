using System.Linq;
using System.Collections.Generic;
using ToSic.Razor.Blade;
public class Helpers : Custom.Hybrid.Code14 {
  public dynamic GetFormulas(string contentType, string fieldName) {
    var contentItem = AsList(App.Data[contentType]).FirstOrDefault();
    var contentItemType = AsEntity(contentItem).Type;
    var fieldType = AsList(contentItemType.Attributes as object).Where(a => a.Name == fieldName).FirstOrDefault();
    
    var attributeMd = AsList(fieldType.Metadata.OfType("@All") as object).FirstOrDefault();
    return AsList(attributeMd.Formulas as object);
  }

  public string Icon() {
    // taken from https://fonts.google.com/icons?icon.query=func
    // Base64 encoded with https://base64.guru/converter/encode/image/svg
    return "svg:PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIGhlaWdodD0iNDgiIHdpZHRoPSI0OCI+PHBhdGggZD0iTTEyIDQwVjM2Ljg1TDI1Ljc1IDI0TDEyIDExLjE1VjhIMzZWMTNIMTkuOEwzMS41NSAyNEwxOS44IDM1SDM2VjQwWiIvPjwvc3ZnPg==";
  }

  public string[] DemoRules(string contentType, string more = null) {
    var newButton = "+new&icon=" + Icon() + "?contentType=" + contentType;
    if (Text.Has(more)) {
      newButton += "&" + more;
    }
    return new [] {
      "toolbar=empty",
      newButton
    };
  }

  public dynamic DemoToolbar(string contentType, string more = null) {
    return Edit.Toolbar(toolbar: DemoRules(contentType, more));
  }
}