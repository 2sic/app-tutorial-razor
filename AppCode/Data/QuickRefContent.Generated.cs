// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "QuickRefContent.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class QuickRefContent
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator v17.07.01
// App/Edition: Tutorial-Razor/
// User:        2sic Web-Developer
// When:        2024-05-13 10:32:09Z
namespace AppCode.Data
{
  // This is a generated class for QuickRefContent 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// QuickRefContent data. <br/>
  /// Generated 2024-05-13 10:32:09Z. Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.Contents`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class QuickRefContent: AutoGenerated.ZAutoGenQuickRefContent
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.QuickRefContent in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZAutoGenQuickRefContent: Custom.Data.CustomItem
  {
    /// <summary>
    /// Contents as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Contents", scrubHtml: true) etc.
    /// </summary>
    public string Contents => _item.String("Contents", fallback: "");

    /// <summary>
    /// Title as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Title", scrubHtml: true) etc.
    /// </summary>
    /// <remarks>
    /// This hides base property Title.
    /// To access original, convert using AsItem(...) or cast to ITypedItem.
    /// Consider renaming this field in the underlying content-type.
    /// </remarks>
    public new string Title => _item.String("Title", fallback: "");
  }
}