// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "UiStringWysiwygButtonMl.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class UiStringWysiwygButtonMl
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v17.08.00
// App/Edition: Tutorial-Razor/
// User:        2sic Web-Developer
// When:        2024-05-21 19:46:38Z
namespace AppCode.Data
{
  // This is a generated class for UiStringWysiwygButtonMl 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// UiStringWysiwygButtonMl data. <br/>
  /// Generated 2024-05-21 19:46:38Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.WysiwygWithExtraButton`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class UiStringWysiwygButtonMl: AutoGenerated.ZAutoGenUiStringWysiwygButtonMl
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.UiStringWysiwygButtonMl in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZAutoGenUiStringWysiwygButtonMl: Custom.Data.CustomItem
  {
    /// <summary>
    /// WysiwygWithExtraButton as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("WysiwygWithExtraButton", scrubHtml: true) etc.
    /// </summary>
    public string WysiwygWithExtraButton => _item.String("WysiwygWithExtraButton", fallback: "");
  }
}