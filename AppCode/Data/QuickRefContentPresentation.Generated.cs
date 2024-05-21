// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "QuickRefContentPresentation.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class QuickRefContentPresentation
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
  // This is a generated class for QuickRefContentPresentation 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// QuickRefContentPresentation data. <br/>
  /// Generated 2024-05-21 19:46:38Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.Color`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class QuickRefContentPresentation: AutoGenerated.ZAutoGenQuickRefContentPresentation
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.QuickRefContentPresentation in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZAutoGenQuickRefContentPresentation: Custom.Data.CustomItem
  {
    /// <summary>
    /// Color as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Color", scrubHtml: true) etc.
    /// </summary>
    public string Color => _item.String("Color", fallback: "");

    /// <summary>
    /// HeadingType as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("HeadingType", scrubHtml: true) etc.
    /// </summary>
    public string HeadingType => _item.String("HeadingType", fallback: "");

    /// <summary>
    /// Highlight as bool. <br/>
    /// To get nullable use .Get("Highlight") as bool?;
    /// </summary>
    public bool Highlight => _item.Bool("Highlight");
  }
}