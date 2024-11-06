// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "UiStringWysiwygMicro.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class UiStringWysiwygMicro
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v18.04.00
// App/Edition: Tutorial-Razor/
// User:        2sic Web-Developer
namespace AppCode.Data
{
  // This is a generated class for UiStringWysiwygMicro 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// UiStringWysiwygMicro data. <br/>
  /// Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.WYSIWYGMicro`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class UiStringWysiwygMicro: AutoGenerated.ZAutoGenUiStringWysiwygMicro
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.UiStringWysiwygMicro in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZAutoGenUiStringWysiwygMicro: Custom.Data.CustomItem
  {
    /// <summary>
    /// WYSIWYGMicro as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("WYSIWYGMicro", scrubHtml: true) etc.
    /// </summary>
    public string WYSIWYGMicro => _item.String("WYSIWYGMicro", fallback: "");
  }
}