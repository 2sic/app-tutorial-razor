// DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
// See also: https://go.2sxc.org/copilot-data
// To extend it, create a "CompositionInherit.cs" with this contents:
/*
namespace AppCode.Data
{
  public partial class CompositionInherit
  {
    // Add your own properties and methods here
  }
}
*/

// Generator:   CSharpDataModelsGenerator
// App/Edition: Tutorial-Razor/
// User:        2sic Web-Developer
namespace AppCode.Data
{
  // This is a generated class for CompositionInherit 
  // To extend/modify it, see instructions above.

  /// <summary>
  /// CompositionInherit data. <br/>
  /// Re-generate whenever you change the ContentType. <br/>
  /// <br/>
  /// Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
  /// Most properties have a simple access, such as `.EMail`. <br/>
  /// For other properties or uses, use methods such as
  /// .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
  /// </summary>
  public partial class CompositionInherit: AutoGenerated.ZAutoGenCompositionInherit
  {  }
}

namespace AppCode.Data.AutoGenerated
{
  /// <summary>
  /// Auto-Generated base class for Default.CompositionInherit in separate namespace and special name to avoid accidental use.
  /// </summary>
  public abstract class ZAutoGenCompositionInherit: Custom.Data.CustomItem
  {
    /// <summary>
    /// EMail as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("EMail", scrubHtml: true) etc.
    /// </summary>
    public string EMail => _item.String("EMail", fallback: "");

    /// <summary>
    /// Name as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Name", scrubHtml: true) etc.
    /// </summary>
    public string Name => _item.String("Name", fallback: "");

    /// <summary>
    /// Salutation as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Salutation", scrubHtml: true) etc.
    /// </summary>
    public string Salutation => _item.String("Salutation", fallback: "");

    /// <summary>
    /// Salutation2 as string. <br/>
    /// For advanced manipulation like scrubHtml, use .String("Salutation2", scrubHtml: true) etc.
    /// </summary>
    public string Salutation2 => _item.String("Salutation2", fallback: "");
  }
}