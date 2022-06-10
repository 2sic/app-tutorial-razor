using System.Linq;
using System.Collections.Generic;
public class Helpers : Custom.Hybrid.Code12 {
  public dynamic GetFormulas(dynamic contentItem, string fieldName) {
    var contentItemType = AsEntity(contentItem).Type;
    var fieldType = AsList(contentItemType.Attributes as object).Where(a => a.Name == fieldName).FirstOrDefault();
    
    var attributeMd = AsList(fieldType.Metadata.OfType("@All") as object).FirstOrDefault();
    return AsList(attributeMd.Formulas as object);
  }
}